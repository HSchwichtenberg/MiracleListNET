# MiracleList .NET
Source code for the MiracleList-Backend (https://miraclelistbackend.azurewebsites.net)
and the Blazor-based implementations of the MiracleList-Clients: Blazor WebAssembly (MiracleList_BW), Blazor Server (MiracleList_BS), Blazor MAUI (MiracleList_BM) and Blazor Desktop (MiracleList_BD)

# URLs to the Live-Systems in the Azure cloud
- Blazor-Server-based Frontend: http://miraclelist-bs.azurewebsites.net
- Blazor-Webassembly-based Frontend: http://miraclelist-bw.azurewebsites.net
- Angular-based Frontend: http://miraclelist.azurewebsites.net
- Vue.js-based Frontend: http://miraclelist-vue.azurewebsites.net
- Backend: http://miraclelistbackend.azurewebsites.net

# Used frameworks and tools
Backend
- .NET with C# 		
- Entity Framework Core 
- SQL Azure
- ASP.NET Core WebAPI 
- Application Insights
- Swagger/Swashbuckle.AspNetCore		
- HTTP tests with Postman

Frontend:
- .NET 
- ASP.NET Core Blazor Server
- ASP.NET Core Blazor Webassembly 
- Windows Presentation Foundation (WPF) with Blazor Desktop 
- .NET MAUI with Blazor MAUI 

# Create the database
- Change the connection string in: DA/Context.cs
- Solution Explorer: Set as Startup Project = EFCTools
- Package Manager Console: Default Project = DA
- Package Manager Console: Run: Update-Database

# Set the connection string to your database
in: MiracleList_BD/appsettings.json
in: MiracleList_BS/appsettings.json
in: MiracleList_Backend/appsettings.json
in: EFC_Tools/appsettings.json
in: Tests/appsettings.json

# Only for for MiracleList_BW and MiracleList_BM running against the existing Cloud-Backend !!!
- Open https://miraclelistbackend.azurewebsites.net/
- Complete the form "Apply for Client ID"
- Get the Client-ID from the Email account
- Set the Client-ID in File MiracleList_BW/wwwRoot/appSettings.json

# Only for MiracleList_BW and MiracleList_BM running against your local Server !!!
1. Start the Server MiracleList_Backend
2. Open the running site in the browser
3. complete the form "Apply for Client ID"
4. Get the Client-ID from the Email account
If you dind't receive an e-mail: Open the table "Client" in the database to get the created Client-ID
5. Set the Client-ID in File MiracleList_BW/wwwRoot/appSettings.json
6. Set the your server-URL in "Backend/DebugURL" in File MiracleList_BW/wwwRoot/appsettings.json

# Execute Unit Tests
- Use Test Explorer

# Test the Website
- View in Browser (STRG SHIFT W)
- http://localhost:8887/index.html