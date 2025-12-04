using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITVisions;
using ITVisions.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MiracleList;
using MiracleList_Backend.Hubs;

namespace MLBlazorRCL.MainView;

enum TaskFilter {  Alle, Offene, Erledigte}

public partial class Main : IAsyncDisposable
{
 public UndoRedoManager UndoRedoManager { get; set; } = new();

 #region DI
 [Inject] AuthenticationStateProvider mLAuthenticationStateProvider { get; set; } = null;
 [Inject] IJSRuntime js { get; set; } = null;
 [Inject] NavigationManager NavigationManager { get; set; } = null;
 [Inject] BlazorUtil Util { get; set; } = null;
 [Inject] Blazored.Toast.Services.IToastService toastService { get; set; }
 [Inject] Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
 [Inject] IAppState AppState { get; set; } = null;
 [Inject] MiracleList.IMiracleListProxy Proxy { get; set; } = null;

 [CascadingParameter]
 protected Task<AuthenticationState> authenticationStateTask { get; set; }
 #endregion

 #region ShouldRender
 private bool shouldRender = true;
 protected override bool ShouldRender()
 {
  return shouldRender;
 }
 #endregion

 #region Parameter
 //[SupplyParameterFromQuery(Name = "t")]
 [Parameter]
 public int TaskID { get; set; } = 0;
 #endregion

 #region Properties zur Datenbindung
 List<BO.Category> categorySet { get; set; }
 List<BO.Task> taskSet { get; set; }
 BO.Category category { get; set; }
 BO.Task task { get; set; }
 string newCategoryName { get; set; }
 string newTaskTitle { get; set; }
 string searchText { get; set; } = "saugen";
 List<BO.Category> searchResultSet { get; set; }
 TaskFilter taskFilter { get; set; } = TaskFilter.Alle;
 #endregion

 #region Komponentenlebenszyklusereignisse
 /// <summary>
 /// Lebenszyklusereignis: Komponente wird initialisiert
 /// </summary>
 /// <returns></returns>
 protected override async Task OnInitializedAsync()
 {
  Util.Log((nameof(Main) + "." + "OnInitializedAsync"));

  // Lade Daten (Kategorieliste)
  await ShowCategorySet();
 }

 protected override async Task OnParametersSetAsync()
 {
  #region Direktansprung einer Aufgabe per URL
  // https://localhost:44387/task/12345
  // oder bei [SupplyParameterFromQuery(Name = "t")]: https://localhost:44387/app?t=12345

  if (TaskID > 0)
  {
   var t = await Proxy.TaskAsync(this.TaskID, AppState.Token);
   if (t is not null)
   {
    this.taskSet = await Proxy.TaskSetAsync(t.CategoryID, AppState.Token);
    await ShowTaskDetail(t);
   }
  }
  #endregion
 }

 protected override async Task OnAfterRenderAsync(bool firstRender)
 {
  if (!firstRender) return; // alles Folgende nur 1x machen

  //toastService.ShowInfo($"Hallo " + AppState.Username + ", herzlich Willkommen bei MiracleList!");

  #region ---- ASP.NET Core SignalR-Verbindung aufbauen
  if (AppState.SignalRHubURL.IsNotNullOrEmpty())
  {

   if (AppState.HubConnection != null && AppState.HubConnection.State == HubConnectionState.Connected) return; // nicht nochmals verbinden, wenn es schon eine bestehende Verbindung gibt!

   var hubURL = new Uri(AppState.SignalRHubURL);
   Util.Log("SignalR.Connecting to " + hubURL.ToString());
   AppState.HubConnection = new HubConnectionBuilder()
       .WithUrl(hubURL)
       .WithServerTimeout(new TimeSpan(0, 0, 40)) // Standard ist 30
       .WithKeepAliveInterval(new TimeSpan(0, 0, 20)) // Standard ist 15
       .AddMessagePackProtocol()
       .WithAutomaticReconnect() // Seit .NET Core 3.1
       .WithStatefulReconnect() // Seit .NET 8.0
       .ConfigureLogging(logging =>
       {
        logging.AddProvider(new ITVisions.Logging.UniversalLoggerProvider(Util.Warn));
        logging.SetMinimumLevel(LogLevel.Debug);
       })
       .Build();

   // Reaktion auf eingehende Nachricht
   AppState.HubConnection.On<string>(nameof(IMLHubV3.CategoryListUpdate), async (sender) =>
   {
    Util.Log($"SignalR.CategoryListUpdate from {sender} (Thread #{System.Threading.Thread.CurrentThread.ManagedThreadId})");
    toastService.ShowSuccess($"Die Aufgabenliste wurde in einer anderen Anwendungsinstanz verändert.");
    await ShowCategorySet(); // Daten neu laden
    await InvokeAsync(StateHasChanged); // InvokeAsync() notwendig hier, weil die Nachricht im Hintergrund (anderer Thread) kommt
   });

   // Reaktion auf eingehende Nachricht
   AppState.HubConnection.On<string, BO.Category>(nameof(IMLHubV3.TaskListUpdate), async (sender, changedCategory) =>
   {
    Util.Log($"SignalR.TaskListUpdate from {sender}: Changed Category={changedCategory.CategoryID} (Thread #{System.Threading.Thread.CurrentThread.ManagedThreadId})");

    if (changedCategory.CategoryID == this.category.CategoryID)
    {
     toastService.ShowSuccess($"Die Aufgabe dieser Kategorie #{category.CategoryID}: \"{this.category.Name}\" wurden in einer anderen Anwendungsinstanz verändert.");
     await ShowTaskSet(this.category); // Daten neu laden
     await InvokeAsync(StateHasChanged); // InvokeAsync() notwendig hier, weil die Nachricht im Hintergrund (anderer Thread) kommt
    }
    else
    {
     if (changedCategory != null) toastService.ShowSuccess($"Die Aufgabe der Kategorie #{changedCategory.CategoryID}: \"{changedCategory.Name}\" wurden in einer anderen Anwendungsinstanz verändert.");
     // Kein UI-Update notwendig
    }
   });

   // Verbindung zum SignalR-Hub starten
   await AppState.HubConnection.StartAsync();
   // Registrieren für Events
   await AppState.HubConnection.SendAsync(nameof(IMLHubV3.Register), AppState.Token);
   Util.Log("SignalR.Connection started!");
  }
  #endregion
 }

 /// <summary>
 /// Beenden der SignalR-Verbindung zum Hub
 /// </summary>
 public async ValueTask DisposeAsync()
 {
  if (AppState.HubConnection != null)
  {
   Util.Log("SignalR.Connection closing...");
   await AppState.HubConnection.StopAsync();
   Util.Log("SignalR.Connection closed!");
  }
 }
 #endregion

 #region Anzeigen von Kategorien und Aufgaben

 /// <summary>
 /// Sorgt für das Ausblenden der Aufgabenliste auf kleinen Bildschirmen
 /// </summary>
 /// <returns></returns>
 public async Task ReturnToCategoryList()
 {
  this.category = null;
 }

 /// <summary>
 /// Lädt die Liste der Kategorien und zeigt die Aufgaben der ersten Kategorie
 /// </summary>
 /// <returns></returns>
 public async Task ShowCategorySet()
 {
  // Lade Daten vom Backend
  categorySet = await Proxy.CategorySetAsync(AppState.Token);
  Util.Log("Loaded Categories: " + categorySet.Count);

  #region Wähle aktuelle Kategorie
  if (this.categorySet.Count > 0)
  {
   if (this.category == null)
   {
    // Kategorie wiederherstellen?
    var storedCategoryID = await localStorage.GetItemAsync<int>("Category");
    if (storedCategoryID > 0)
    {
     // ist die gespeicherte Kategorie eine aktuelle Kategorie?
     var storedCategory = this.categorySet.SingleOrDefault(x => x.CategoryID == storedCategoryID);
     if (storedCategory != null)
     {
      Util.Log("Restore Category", storedCategoryID);
      await ShowTaskSet(storedCategory);
     }
    }
   }

   // immer noch keine Kategorie gewählt? dann nimm die erste
   if (this.category == null)
   {
    await ShowTaskSet(categorySet[0]);
   }
  }
  #endregion
 }

 public async Task ShowTaskSet(BO.Category c)
 {
  if (c == null) { return; }
  Util.Log(nameof(ShowTaskSet) + ": " + c.CategoryID + " (" + c.Name + ")");
  // eventuelle Suchergebnisse ausblenden
  this.searchResultSet = null;
  this.searchText = "";
  // aktuelle Kategorie setzen
  this.category = c;
  // Aufgaben zu dieser Kategorie laden
  this.taskSet = await Proxy.TaskSetAsync(c.CategoryID, AppState.Token);
  // Filter im RAM anwenden, da Backend diese Filter nicht bietet
  if (this.taskFilter == TaskFilter.Offene) this.taskSet = this.taskSet.Where(x => x.Done == false).ToList();
  if (this.taskFilter == TaskFilter.Erledigte) this.taskSet = this.taskSet.Where(x => x.Done == true).ToList();

  // aktuelle Kategorie im Local Storage merken für den Fall eines Reloads
  await localStorage.SetItemAsync<int>("Category", c.CategoryID);
  // Kein Task soll ausgewählt sein
  this.task = null;
 }

 public async Task ShowTaskDetail(BO.Task t)
 {
  Util.Log(nameof(ShowTaskDetail) + ": " + t.TaskID);
  this.task = t;
  if (t.Category != null) this.category = t.Category; // notwendig, falls Suchergebnisse gezeigt werden
 }

 public async Task ReloadTaskList()
 {
  // Deaktivieren von ShouldRender führt dazu, dass es kein Flackern gibt, bei dem man kurz die abgebrochene Änderung aus <TaskEdit> hier sieht
  shouldRender = false;
  await ShowTaskSet(this.category);
  shouldRender = true;
  // Kein Task soll ausgewählt sein
  this.task = null;
 }
 #endregion

 #region Erstellen von neuen Kategorien und Aufgaben
 /// <summary>
 /// Use Keyup instead of Keypress as the actual data binding did not yet happen when Keypress is fired
 /// </summary>
 public async Task NewCategory_Keyup(KeyboardEventArgs e)
 {
  if (e.Key == "Enter")
  {
   Util.Log("CreateCategory: " + this.newCategoryName);
   if (!String.IsNullOrEmpty(this.newCategoryName)) await CreateCategory(this.newCategoryName);
  }
 }

 /// <summary>
 /// Use Keyup instead of Keypress as the actual data binding did not yet happen when Keypress is fired
 /// </summary>
 public async Task NewTask_Keyup(KeyboardEventArgs e)
 {
  if (e.Key == "Enter")
  {
   Util.Log("CreateTask: " + this.newTaskTitle);
   if (!String.IsNullOrEmpty(this.newTaskTitle)) await CreateTask(this.newTaskTitle);
   newTaskTitle = "";
  }
 }

 public async Task CreateCategory(string newCategoryName)
 {
  if (string.IsNullOrEmpty(newCategoryName)) return;
  Util.Log("createCategory: " + newCategoryName);
  var newcategory = await Proxy.CreateCategoryAsync(newCategoryName, AppState.Token);

  UndoRedoManager.Create("Kategorie angelegt: " + this.newCategoryName,
   async () =>
   {
    await Proxy.CreateCategoryAsync(newCategoryName, AppState.Token);
    await ShowCategorySet();
   },
   async () =>
   {
    await Proxy.DeleteCategoryAsync(newcategory.CategoryID, AppState.Token);
    await ShowCategorySet();
   });

  this.categorySet.Add(newcategory);
  await ShowTaskSet(newcategory);
  this.newCategoryName = "";
  await SendCategoryListUpdate(); // SignalR
 }

 public async Task CreateTask(string newTaskTitle)
 {
  if (string.IsNullOrEmpty(newTaskTitle)) return;
  Util.Log("createTask: " + newTaskTitle + " in category: " + this.category.Name);
  var t = new BO.Task();
  t.TaskID = 0; // ID wird vom BL/Backend vergeben!
  t.Title = newTaskTitle;
  t.CategoryID = this.category.CategoryID;
  t.Importance = BO.Importance.B;
  t.Created = DateTime.Now;
  t.Due = null;
  t.Order = 0;
  t.Note = "";
  t.Done = false;
  // BL/Backend liefert den Task zurück mit der korrekten ID
  t = await Proxy.CreateTaskAsync(t, AppState.Token);

  // UI Update
  this.newTaskTitle = "";
  this.taskFilter = TaskFilter.Alle;
  await ShowTaskSet(this.category);

  // SignalR
  await SendTaskListUpdate();
 }
 #endregion

 #region Entfernen und Ändern 

 DotNetObjectReference<Main> obj;
 /// <summary>
 /// Ereignisbehandlung: Benutzer löscht Kategorie
 /// </summary>
 public async System.Threading.Tasks.Task RemoveCategory(BO.Category c)
 {
  obj = DotNetObjectReference.Create(this);
  await Util.ConfirmDialog("Löschen der Kategorie #" + c.CategoryID + " mit " + c.TaskSet.Count + " Aufgaben?", c.CategoryID, obj, ConfirmedRemoveCategory);
 }

 /// <summary>
 /// Ereignisbehandlung: Benutzer löscht Kategorie
 /// </summary>
 [JSInvokable]
 public async Task ConfirmedRemoveCategory(int categoryID, bool result)
 {
  if (result == false) return;
  // Löschen via WebAPI-Aufruf
  await Proxy.DeleteCategoryAsync(categoryID, AppState.Token);

  var cat = categorySet.FirstOrDefault(c => c.CategoryID == categoryID);
  UndoRedoManager.Create("Kategorie gelöscht: " + cat.Name,
  async () =>
  {
   await Proxy.DeleteCategoryAsync(categoryID, AppState.Token);
   await ShowCategorySet();
  },
  async () => { 
   await Proxy.CreateCategoryAsync(cat.Name, AppState.Token);
   await ShowCategorySet();
  });

  // aktuelle Category zurücksetzen
  this.category = null;
  this.taskSet = null;
  // Liste der Kategorien neu laden
  await ShowCategorySet();
  // UI-Update erzwingen
  await InvokeAsync(this.StateHasChanged);
  // Benachrichtigung an alle Clients via ASP.NET Core SignalR
  await SendCategoryListUpdate();
 }

 /// <summary>
 /// wird gerufen, wenn <TaskEdit> fertig ist
 /// </summary>
 /// <param name="save">true = Änderung soll gespeichert werden</param>
 public async Task TaskHasChanged(bool save)
 {
  Util.Log(nameof(TaskHasChanged) + ": saved=" + save);
  // reload all tasks in current category
  if (save)
  {
   await Proxy.ChangeTaskAsync(this.task, AppState.Token);
   toastService.ShowSuccess($"Aufgabe #" + task.TaskID + " wurde gespeichert.");
   await SendTaskListUpdate(task);
   this.task = null;
  }
  else
  {
   await ReloadTaskList();
  }
 }
 #endregion

 #region Suche
 /// <summary>
 /// Use Keyup instead of Keypress as the actual data binding did not yet happen when Keypress is fired
 /// </summary>
 public async Task Search(KeyboardEventArgs e)
 {
  Util.Log(e.Key);
  if (e.Key == "Enter")
  {
   Util.Log("Search: " + this.searchText);
   if (!String.IsNullOrEmpty(this.searchText))
   {
    this.category = null;
    this.searchResultSet = await Proxy.SearchAsync(this.searchText, AppState.Token);
   }
  }
 }
 #endregion

 #region SignalR-Nachrichten senden
 /// <summary>
 /// SignalR-Connection OK?
 /// </summary>
 public bool IsConnected =>
  AppState.HubConnection != null ? AppState.HubConnection.State == HubConnectionState.Connected : false;

 /// <summary>
 /// Sende Nachricht an Hub via ASP.NET SignalR
 /// </summary>
 public async Task SendCategoryListUpdate()
 {
  Util.Log($"SignalR.{nameof(SendCategoryListUpdate)}");
  // DEMO: 61. SignalR-Event auslösen
  if (IsConnected) await AppState.HubConnection.SendAsync(nameof(IMLHubV3.CategoryListUpdate), AppState.Token);
  else Util.Warn($"SignalR.{nameof(SendCategoryListUpdate)}: not connected!", "");
 }

 /// <summary>
 /// Sende Nachricht an Hub via ASP.NET SignalR
 /// </summary>
 public async Task SendTaskListUpdate(BO.Task t = null)
 {
  BO.Category categoryUpdated;

  // TODO: hier gibt es ein Serialisierungs-StackOverflow-Problem, wenn man die Kategorie inklusive TaskList übermittelt per SignalR
  if (t != null) { categoryUpdated = t.Category; }
  else { categoryUpdated = this.category; }
  Util.Log($"SignalR.{nameof(SendTaskListUpdate)}: {categoryUpdated.CategoryID}");

  if (IsConnected) await AppState.HubConnection.SendAsync(nameof(IMLHubV3.TaskListUpdate), AppState.Token, categoryUpdated);
  else Util.Warn($"SignalR.{nameof(SendTaskListUpdate)}: not connected!", "");
 }
 #endregion

 #region Drag&Drop
 public BO.Task taskInDragAndDrop = null;
 public void DragTask(DragEventArgs e, BO.Task task)
 {
  Util.Log("DragTask", task);
  taskInDragAndDrop = task;
 }

 public async Task DropTaskToCategory(DragEventArgs e, BO.Category category)
 {
  if (taskInDragAndDrop == null) return;
  Util.Log("DropTaskToCategory", taskInDragAndDrop, category);
  taskInDragAndDrop.CategoryID = category.CategoryID;
  await Proxy.ChangeTaskAsync(taskInDragAndDrop, AppState.Token);
  await ShowTaskSet(this.category);
  taskInDragAndDrop = null;
 }
 #endregion

 private async Task Redo(MouseEventArgs args)
 {
  UndoRedoManager.Redo();
 }

 private async Task Undo(MouseEventArgs args)
 {
  UndoRedoManager.Undo();
 }

 private async Task TaskRemoved(BO.Task t)
 {
  UndoRedoManager.Create("Aufgabe gelöscht: " + t.Title,
  async () =>
  {
   Util.Log("Redo: Aufgabe #" + t.TaskID + " löschen!");
   await Proxy.DeleteTaskAsync(t.TaskID, AppState.Token);
   await ShowTaskSet(category);
   await this.SendTaskListUpdate();
  },
  async () =>
  {
   Util.Log("Undo: Aufgabe #" + t.TaskID + " gelöscht!");
   t.TaskID = 0; // damit Backend neue ID vergibt
   t = await Proxy.CreateTaskAsync(t, AppState.Token);
   var category = this.categorySet.FirstOrDefault(x => x.CategoryID == t.CategoryID);
   await ShowTaskSet(category);
   await this.SendTaskListUpdate();
  });
  await this.SendTaskListUpdate();
  await ReloadTaskList();
 }
} // end class Main