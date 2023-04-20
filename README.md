## Über MiracleList

MiracleList ist eine umfangreiche, sehr realitätsnahe Beispielanwendung, die eine ähnliche Benutzeroberfläche und Funktionen wie das inzwischen leider eingestellte [Wunderlist](https://de.wikipedia.org/wiki/Wunderlist) bietet. Diese Beispielanwendung verwendet [Dr. Holger Schwichtenberg](https://www.dotnet-doktor.de) in seinen [Fachbüchern](https://www.IT-Visions.de/Verlag), [Schulungen](https://www.IT-Visions.de/Schulungen) und [Vorträgen](https://www.IT-Visions.de/Vortraege).

<div class="alert alert-info">Während in vielen Veröffentlichungen und Weiterbildungsmaßen immer nur einzelne kleine, aus dem Kontext gerissene Beispiele gezeigt werden, dient MiracleList dazu, Softwareentwicklern möglichst viele Funktionen in einem praxisnahen Gesamtzusammenhang zu zeigen.</div>

## Vier Blazor-Implementierungen der MiracleList

Es gibt vier auf ASP.NET Core Blazor basierende Implementierung des MiracleList-Frontends:

*   **MiracleList_BW**: MiracleList mit Blazor WebAssembly (3-Tier mit Zugriff auf die Daten über WebAPI-basierten Application Server)
*   **MiracleList_BS**: MiracleList mit Blazor Server (2-Tier mit Direktzugriff auf die Datenbank)
*   **MiracleList_BD**: MiracleList mit Blazor Desktop in einer WPF-Rahmenanwendung (2-Tier mit Direktzugriff auf die Datenbank)
*   **MiracleList_BM**: MiracleList mit Blazor MAUI (3-Tier mit Zugriff auf die Daten über WebAPI-basierten Application Server)

![image](https://user-images.githubusercontent.com/3673169/224552620-97f7c195-f365-4b67-80e7-cbf3fd98c34f.png)

Abbildung 1: MiracleList im Browser mit Blazor WebAssembly

![](https://user-images.githubusercontent.com/3673169/224502120-1e4a7310-b574-49f5-b7dd-72b240f9fe92.png)

Abbildung 2: MiracleList als hybride App in Blazor MAUI

## Architektur der Blazor-Implementierungen der MiracleList

Die vier Blazor-Implementierungen der MiracleList verwenden sehr viel gemeinsamen Code. Die Benutzeroberfläche ist nur einmal für alle vier Blazor-Implementierungen der MiracleList realisiert. Der Datenzugriffs erfolgt wahlweise direkt auf die Datenbank (2-Tier) oder via WebAPI/Application Server (3-Tier).

![image](https://user-images.githubusercontent.com/3673169/227788424-d7ff21be-3770-41ff-9cbd-54f52996df2b.png)
Abbildung 3: Code-Sharing zwischen den vier Implementierungen einschließlich Abstraktion von 2-Tier/3-Tier

## Features der Blazor-Implementierung der MiracleList

Welche Funktionen die Blazor-Implementierungen der MiracleList aus Benutzersicht bieten, finden Sie in der Tabelle auf [www.MiracleList.net](http://www.MiracleList.net)

Aus technischer Sicht demonstrieren die Blazor-Implementierungen der MiracleList für Softwareentwickler folgende Funktionen von Blazor:

*   Alle vier Blazor-Varianten (Blazor WebAssembly,Blazor Server,Blazor Desktop, Blazor MAUI) für Browser und als Hybridanwendung
*   Sehr viel Shared Code zwischen allen Blazor-Varianten mit einem gemeinsamen UI in einer Razor Class Library
*   Abstraktion zwischen einer 2-Tier- und 3-Tier-Anwendung: Mit dem gleichen Komponenten sowohl direkt auf eine Datenbank zugreifen als auch Nutzung von WebAPIs auf einem Application Server
*   Listenansichten mit Suchen und Filtern
*   Bearbeitungsformular mit Validierung
*   Datei-Upload
*   Tastaturereignisse
*   Editierbares Datagrid
*   Drag&Drop
*   Kontextmenüs
*   Modale Dialoge
*   Toast-Benachrichtigungen
*   Ständig aktualisierte Statusanzeige für Backendsysteme
*   Generierung eines Berichts via Microsoft Word (nur in Hybridanwendung)
*   Authentifizierung und Autorisierung
*   Nutzung des Local Storage des Browsers
*   Fenster-Synchronisation mit Push-Nachrichten via ASP.NET Core SignalR
*   Progressive Web App (nur Blazor WebAssembly)
*   Einsatz kostenfreier Zusatzkomponenten

## Verwendete Frameworks und Tools
Backend:
- .NET mit C#
- Entity Framework Core
- SQL Azure
- ASP.NET Core-WebAPI
- Swagger/Swashbuckle.AspNetCore
- HTTP-Tests mit Postman

Frontend:
- .NET
- ASP.NET Core Blazor Server
- ASP.NET Core Blazor Webassembly
- Windows Presentation Foundation (WPF) mit Blazor Desktop
- .NET MAUI mit Blazor MAUI

## Entwicklerdokumentation

Die Entwicklerdokumentation zu den vier MiracleList-Frontends mit Blazor finden im <a href="https://it-visions.de/blazorbuch">Blazor-Buch von Dr. Holger Schwichtenberg</a>.

<a href="https://it-visions.de/blazorbuch">
<img src="https://user-images.githubusercontent.com/3673169/224503307-5dcda1a8-612b-4ee6-95e8-2dad43fa917d.png" width="300">
</a>

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

## Erste Schritte zur Einrichtung auf Ihrem Entwickler-PC

### Erstellen Sie die Datenbank (nur für Backend und 2-Tier-Varianten notwendig!)
- Ändern Sie die Verbindungszeichenfolge in: DA/Context.cs
- Projektmappen Explorer: Als Startprojekt festlegen = EFCTools
- Package Manager Console: Standardprojekt = DA
- Package Manager Console: Ausführen: Update-Database

### Setzen Sie die Verbindungszeichenfolge auf Ihre Datenbank (nur für Backend und 2-Tier-Varianten notwendig!)
- in: MiracleList_BD/appsettings.json
- in: MiracleList_BS/appsettings.json
- in: MiracleList_Backend/appsettings.json
- in: EFC_Tools/appsettings.json
- in: Tests/appsettings.json

### Nur für MiracleList_BW und MiracleList_BM, wenn Sie das vorhandene Cloud-Backend verwenden wollen
- Öffnen Sie https://miraclelistbackend.azurewebsites.net/
- Füllen Sie das Formular „Kunden-ID beantragen“ aus
- Holen Sie sich die Client-ID aus dem E-Mail-Konto
- Setzen Sie die Client-ID in der Datei MiracleList_BW/wwwRoot/appSettings.json

### Nur für MiracleList_BW und MiracleList_BM, wenn diese auf Ihrem lokalen Server laufen
1. Starten Sie den Server MiracleList_Backend
2. Öffnen Sie die laufende Site im Browser
3. Füllen Sie das Formular „Kunden-ID beantragen“ aus
4. Holen Sie sich die Client-ID aus dem E-Mail-Konto. Wenn Sie keine E-Mail erhalten haben: Öffnen Sie die Tabelle "Client" in der Datenbank, um die erstellte Client-ID zu erhalten
5. Legen Sie die Client-ID in der Datei MiracleList_BW/wwwRoot/appSettings.json fest
6. Setzen Sie Ihre Server-URL in „Backend/DebugURL“ in der Datei MiracleList_BW/wwwRoot/appsettings.json

## Weitere Informationen
- Website zum MiracleList-Projekt: [www.miraclelist.de](http://www.miraclelist.de)
