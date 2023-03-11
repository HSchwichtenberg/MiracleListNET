<h2>�ber MiracleList</h2>
<p>
 MiracleList ist eine umfangreiche, sehr realit�tsnahe Beispielanwendung, die �hnliche Konzepte wie das inzwischen leider eingestellte <a href="https://de.wikipedia.org/wiki/Wunderlist">Wunderlist</a> bietet. Diese Beispielanwendung verwendet <a href="https://www.dotnet-doktor.de">Dr. Holger Schwichtenberg</a> in seinen <a href="https://www.IT-Visions.de/Verlag">Fachb�chern</a>, <a href="https://www.IT-Visions.de/Schulungen">Schulungen</a> und <a href="https://www.IT-Visions.de/Vortraege">Vortr�gen</a>. 
 
 <div class="alert alert-info">
  W�hrend in vielen Ver�ffentlichungen und Weiterbildungsma�en immer nur einzelne kleine, aus dem Kontext gerissene Beispiele gezeigt werden, dient MiracleList dazu, Softwareentwicklern m�glichst viele Funktionen in einem praxisnahen Gesamtzusammenhang zu zeigen.
 </div>
</p>

<h2>Vier Blazor-Implementierungen der MiracleList</h2>

 Es gibt vier auf ASP.NET Core Blazor basierende Implementierung des MiracleList-Frontends:
<ul>
 <li><b>MiracleList_BW</b>: MiracleList mit Blazor WebAssembly (3-Tier mit Zugriff auf die Daten �ber WebAPI-basierten Application Server)</li>
 <li><b>MiracleList_BS</b>: MiracleList mit Blazor Server (2-Tier mit Direktzugriff auf die Datenbank)</li>
 <li><b>MiracleList_BD</b>: MiracleList mit Blazor Desktop in einer WPF-Rahmenanwendung (2-Tier mit Direktzugriff auf die Datenbank)</li>
 <li><b>MiracleList_BM</b>: MiracleList mit Blazor MAUI (3-Tier mit Zugriff auf die Daten �ber WebAPI-basierten Application Server)</li>
</ul>

<h2>Features der Blazor-Implementierung der MiracleList</h2>

<p>Welche Funktionen die Blazor-Implementierungen der MiracleList aus Benutzersicht bieten, finden Sie in der Tabelle auf <a href="http://www.MiracleList.net">www.MiracleList.net</a></p>

<p>Aus technischer Sicht demonstrieren die Blazor-Implementierungen der MiracleList f�r Softwareentwickler folgende Funktionen von Blazor </p>
<ul>
 <li>Alle vier Blazor-Varianten (Blazor WebAssembly,Blazor Server,Blazor Desktop, Blazor MAUI) f�r Browser und als Hybridanwendung</li>
 <li>Sehr viel Shared Code zwischen allen Blazor-Varianten mit einem gemeinsamen UI in einer Razor Class Library </li>
 <li>Abstraktion zwischen einer 2-Tier- und 3-Tier-Anwendung: Mit dem gleichen Komponenten sowohl direkt auf eine Datenbank zugreifen als auch Nutzung von WebAPIs auf einem Application Server</li>
 <li>Listenansichten mit Suchen und Filtern</li>
 <li>Bearbeitungsformular mit Validierung</li>
 <li>Datei-Upload</li>
 <li>Tastaturereignisse</li>
 <li>Editierbares Datagrid</li>
 <li>Drag&Drop</li>
 <li>Kontextmen�s</li>
 <li>Modale Dialoge</li>
 <li>Toast-Benachrichtigungen</li>
 <li>St�ndig aktualisierte Statusanzeige f�r Backendsysteme</li>
 <li>Generierung eines Berichts via Microsoft Word (nur in Hybridanwendung)</li>
 <li>Authentifizierung und Autorisierung</li>
 <li>Nutzung des Local Storage des Browsers</li>
 <li>Fenster-Synchronisation mit Push-Nachrichten via ASP.NET Core SignalR</li>
 <li>Progressive Web App (nur Blazor WebAssembly)</li>
</ul>

<h2>MiracleList-Live-Systeme in der Cloud</h2>
<ul>
 <li>Backend mit C#/ASP.NET Core: <a href="https://miraclelistbackend.azurewebsites.net" rel="nofollow">https://miraclelistbackend.azurewebsites.net</a></li>
 <li>Web-Frontend C#/Blazor Server: <a href="https://miraclelist-bs.azurewebsites.net" rel="nofollow">https://miraclelist-bs.azurewebsites.net</a></li>
 <li>Web-Frontend C#/Blazor WebAssembly: <a href="https://miraclelist-bw.azurewebsites.net" rel="nofollow">https://miraclelist-bw.azurewebsites.net</a></li>
 <li>Web-Frontend TypeScript/Vue.js: <a href="https://miraclelist-vue.azurewebsites.net/" rel="nofollow">https://miraclelist-vue.azurewebsites.net/</a></li>
 <li>Web-Frontend TypeScript/Angular: <a href="http://www.miraclelist.net" rel="nofollow">http://www.miraclelist.net</a></li>

 <li>Windows-Client TypeScript/Angular/Electron: <a href="https://Miraclelist.azurewebsites.net/download/MiracleListElectron-win32-x64.rar" rel="nofollow">https://Miraclelist.azurewebsites.net/download/MiracleListElectron-win32-x64.rar</a></li>
 <li>MacOS-Client TypeScript/Angular/Electron: <a href="https://Miraclelist.azurewebsites.net/download/MiracleListElectron-darwin-x64.rar" rel="nofollow">https://Miraclelist.azurewebsites.net/download/MiracleListElectron-darwin-x64.rar</a></li>
 <li>Linux-Client TypeScript/Angular/Electron: <a href="https://Miraclelist.azurewebsites.net/download/MiracleListElectron-linux-x64.rar" rel="nofollow">https://Miraclelist.azurewebsites.net/download/MiracleListElectron-linux-x64.rar</a></li>
</ul>

## Verwendete Frameworks und Tools
Backend:
- .NET mit C#
- Entity Framework-Kern
- SQL-Azure
- ASP.NET Core-WebAPI
- Anwendungseinblicke
- Swagger/Swashbuckle.AspNetCore
- HTTP-Tests mit Postman

Frontend:
- .NETZ
- ASP.NET Core Blazor-Server
- ASP.NET Core Blazor-Webassembly
- Windows Presentation Foundation (WPF) mit Blazor Desktop
- .NET-MAUI mit Blazor-MAUI

## Erstellen Sie die Datenbank
- �ndern Sie die Verbindungszeichenfolge in: DA/Context.cs
- Projektmappen-Explorer: Als Startprojekt festlegen = EFCTools
- Paket-Manager-Konsole: Standardprojekt = DA
- Paket-Manager-Konsole: Ausf�hren: Update-Database

## Setzen Sie die Verbindungszeichenfolge auf Ihre Datenbank
- in: MiracleList_BD/appsettings.json
- in: MiracleList_BS/appsettings.json
- in: MiracleList_Backend/appsettings.json
- in: EFC_Tools/appsettings.json
- in: Tests/appsettings.json

## Nur f�r MiracleList_BW und MiracleList_BM, die gegen das vorhandene Cloud-Backend laufen !!!
- �ffnen Sie https://miraclelistbackend.azurewebsites.net/
- F�llen Sie das Formular �Kunden-ID beantragen� aus
- Holen Sie sich die Client-ID aus dem E-Mail-Konto
- Setzen Sie die Client-ID in der Datei MiracleList_BW/wwwRoot/appSettings.json

## Nur f�r MiracleList_BW und MiracleList_BM, die auf Ihrem lokalen Server laufen !!!
1. Starten Sie den Server MiracleList_Backend
2. �ffnen Sie die laufende Site im Browser
3. F�llen Sie das Formular �Kunden-ID beantragen� aus
4. Holen Sie sich die Client-ID aus dem E-Mail-Konto. Wenn Sie keine E-Mail erhalten haben: �ffnen Sie die Tabelle "Client" in der Datenbank, um die erstellte Client-ID zu erhalten
5. Legen Sie die Client-ID in der Datei MiracleList_BW/wwwRoot/appSettings.json fest
6. Setzen Sie Ihre Server-URL in �Backend/DebugURL� in der Datei MiracleList_BW/wwwRoot/appsettings.json

<h2>Weitere Informationen</h2>
<ul>
 <li>Website zum MiracleList-Projekt: <a href="http://www.miraclelist.de" rel="nofollow">www.miraclelist.de</a></li>
</ul>