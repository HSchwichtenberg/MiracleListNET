
Cd $PSScriptRoot\src\EFCTools\bin\debug\netcoreapp2.0

$env:ConnectionStrings:MiracleListDB = "Data Source=localhost;Initial Catalog=MiracleList_TEMP2;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

dotnet .\EFCTools.dll migrate 


#dotnet .\EFCTools.dll recreate 