#nullable enable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ITVisions;

namespace BO;

/// <summary>
/// Klasse für den Fragebogen mit Personendaten, Benutzerkonto und Feedback
/// </summary>
public class Fragebogen
{

 #region Personendaten - Name der Person
 [Chapter("Personendaten")]
 [Headline("Name der Person")]
 public string PersonendatenText { get; } = "Wir beginnen damit, dass wir einige **elementare Personendaten** abfragen. Damit ist nur der **Name** ein Pflichteld.";

 /// <summary>
 /// Anrede der Person
 /// </summary>
 [Display(Name = "Anrede", Description = "Aufzählungstypen werden zu Auswahlfeldern.", GroupName = "")]
 public AnredeEnum? Anrede { get; set; }

 /// <summary>
 /// Titel der Person (z.B. Dr., Prof.)
 /// </summary>
 [Display(Name = "Titel", Description = "Hier wurden die Auswahlfelder nicht via Aufzählungstypen, sondern durch eine Annotation festgelegt.", GroupName = "")]
 [Select("", "Dr.", "Prof.", "Mag.")]
 public string? Titel { get; set; }

 /// <summary>
 /// Vorname der Person
 /// </summary>
 [Label("Vorname", "Aus string wird ein einfaches Textfeld.")]
 [Group("Name der Person")]
 public string? Vorname { get; set; }

 /// <summary>
 /// Name der Person (Pflichtfeld)
 /// </summary>
 [Group("Name der Person")]
 [Required(ErrorMessage = "Nachname ist ein Pflichtfeld")]
 [Label("Nachname", "Das Property ist nicht nullable, daher ist Nachname ein Pflichtfeld.")]
 public string Nachname { get; set; } = string.Empty;
 #endregion

 #region Weitere Personendaten

 [Headline("Weitere Personendaten")]
 public string WeiterePersonendatenText { get; } = "Bitte erlauben Sie uns, noch *etwas* mehr über Sie zu erfahren. Hierzu kommen weitere Felddatentypen zum Einsatz.";
 /// <summary>
 /// Geburtsdatum der Person
 /// </summary>

 [Display(Name = "Geburtsdatum", GroupName = "Daten zu Ihrer Geburt")]
 [DataType(DataType.Date)]
 public DateTime Geburtsdatum { get; set; }

 /// <summary>
 /// Geburtsuhrzeit der Person
 /// </summary>
 [Display(Name = "Geburtsuhrzeit", GroupName = "Daten zu Ihrer Geburt")]
 [DataType(DataType.Time)]
 public TimeSpan? Geburtsuhrzeit { get; set; }

 /// <summary>
 /// Anzahl der Kinder (0-10)
 /// </summary>
 [Group("Ihre Kinder")]
 [Label("Kinder", "Ihre Kinder")]
 [Range(0, 10)]
 public int? Kinder { get; set; }

 /// <summary>
 /// Alter des ältesten Kindes (0-100)
 /// </summary>
 [Group("Ihre Kinder")]
 [Label("Alter des ältesten Kindes", "Zwischen 0 und 100")]
 [Range(0, 100)]
 public int AlterKind1 { get; set; } = 20;

 /// <summary>
 /// Alter des jüngsten Kindes (0-100)
 /// </summary>
 [Group("Ihre Kinder")]
 [Label("Alter des jüngsten Kindes", "Zwischen 0 und 100")]
 [Range(0, 100)]
 public int AlterKind2 { get; set; } = 10;

 /// <summary>
 /// Lebensmotto der Person
 /// </summary>

 [Label("Lebensmotto", "Der Vorgabewert bei diesem Property wird berücksichtigt.")]
 public string Lebensmotto { get; set; } = "Weniger ist mehr.";

 /// <summary>
 /// Hobbys der Person (Mehrfachauswahl)
 /// </summary>
 [MultiSelect]
 [Label("Hobbys", "[Multiselect] sorgt für eine Mehrfachauswahl. Auch hier sind Vorgabewerte möglich.")]
 public HobbyEnum Hobbys { get; set; } = HobbyEnum.MTB | HobbyEnum.Reisen;

 /// <summary>
 /// Telefonnummer der Person
 /// </summary>
 [Label("Telefonnummer", "Durch die Annotation [Phone] sind nur Zahlen und einige Sonderzeichen (+, -, /, Leerzeichen, Klammern), aber keine Buchstaben erlaubt.")]
 [Phone]
 public string? Telefonnummer { get; set; }

 /// <summary>
 /// Website der Person
 /// </summary>
 [Label("Website", "Bei [URL] prüft der Browser, ob es sich um eine Webadresse handelt.")]
 [Url]
 public string? Website { get; set; }

 /// <summary>
 /// Familienstand der Person
 /// </summary>
 [RadioButtonList]
 [Label("Status", "Die Annotation [RadioButtonList] erzwingt eine RadioButtonList ;-).")]
 public FamilienstandEnum Status { get; set; } = FamilienstandEnum.Ledig;

 /// <summary>
 /// Newsletter-Anmeldung
 /// </summary>
 [Label("Newsletter", "Aus einem booleschen Wert wird eine Checkbox.", Details = "Newsletter abonnieren", Description = "Unser Newsletter erscheint monatlich. Sie können jederzeit den Newsletter kündigen.")]
 public bool Newsletter { get; set; } = true;

 //[Hidden]
 [ReadOnly(true)]
 [Label("Ihre automatisch vergebene ID", "Durch die Annotation [ReadOnly(true)] wird dieses Property im Formular nicht bearbeitbar, aber sichtbar. Es könnte z.B. als eindeutige ID für die Person dienen.")]
 public Guid PersonID { get; set; } = Guid.CreateVersion7();

 #endregion

 #region Benutzerkonto

 /// <summary>
 /// Kundennummer (Format: K-xxxx mit 4 Ziffern)
 /// </summary>
 [Chapter("Benutzerkonto")]
 [Required]
 [Label("Kundennummer", "Validierung mit regulären Ausdruck: Die Kundennummer muss im Format K-xxxx sein (z.B. K-1234)")]
 [RegularExpression(@"K-([0-9]{4})")]
 public string? Kundennummer { get; set; }

 /// <summary>
 /// E-Mail-Adresse des Benutzers
 /// </summary>
 [Label("E-Mail", "Die Annotation [EmailAddress] prüft, ob es sich um eine gültige E-Mail-Adresse handelt.")]
 [Required]
 [EmailAddress]
 public string? Email { get; set; }

 /// <summary>
 /// Kennwort des Benutzers
 /// </summary>
 [Label("Kennwort", "[DataType(DataType.Password)] sorgt dafür, dass man bei der Eingabe nur Sternchen sieht. Es gibt keine eingebauten Regeln für die Kennwortlänge und -komplexität; man kann aber zusätzlich einen regulären Ausdruck angeben.")]
 [DataType(DataType.Password)]
 [Required]
 //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Das Kennwort muss mindestens 8 Zeichen lang sein und mindestens einen Großbuchstaben, einen Kleinbuchstaben, eine Zahl und ein Sonderzeichen enthalten. Diese Prüfung erfolgt per regulärem Ausdruck.")]
 public string? Kennwort { get; set; }

 #endregion

 #region Feedback

 /// <summary>
 /// Bewertung der Anwendung (1-5 Sterne)
 /// </summary>
 [Chapter("Feedback")]
 [Headline("Bewertung mit Skala")]
 [Label("Bewertung1 (1 bis 5)", "Bewerten Sie diese Anwendung")]
 [Range(1, 5)]
 public int? Bewertung1 { get; set; }

 /// <summary>
 /// Bewertung der Anwendung (Skala 1-5)
 /// </summary>
 [Label("Bewertung2 (1 bis 5)", "Bewerten Sie diese Anwendung")]
 [Range(1, 5)]
 public int Bewertung2 { get; set; } = 3;

 /// <summary>
 /// Freitextkommentar zur Bewertung
 /// </summary>
 [Headline("Freitextbewertung")]
 [Label("Kommentar", "Eine [DataType(DataType.MultilineText)] erzeugt ein mehrzeiliges Textfeld. Die Anzahl der Zeile legt man via [Size] fest.")]
 [DataType(DataType.MultilineText)]
 [Size(8)]
 [MaxLength(2000)]
 public string? Kommentar { get; set; }
 #endregion
}