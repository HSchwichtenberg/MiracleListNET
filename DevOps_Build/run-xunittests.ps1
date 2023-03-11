cls
"Start Unit tests for " + $PSScriptRoot

$ENVName = "ConnectionStrings:MiracleListDB"
#$CS = "Data Source=.;Initial Catalog=MiracleList_Test;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
#$CS = "" # l�schen!
$CS = "InMemoryDB"

"Set ConnectionStrings:MiracleListDB = $CS ..."
#[Environment]::SetEnvironmentVariable("ConnectionStrings:MiracleListDB", $CS, "Machine")
#[Environment]::SetEnvironmentVariable("ConnectionStrings:MiracleListDB", $CS, "User")
$env:ConnectionStrings:MiracleListDB = $CS

"Environment Process: " + $env:ConnectionStrings:MiracleListDB
"Environment User: " + [Environment]::GetEnvironmentVariable("ConnectionStrings:MiracleListDB","User")
"Environment Machine: " + [Environment]::GetEnvironmentVariable("ConnectionStrings:MiracleListDB","Machine")

"Starting tests..."
dotnet test $PSScriptRoot\Test\UnitTests\UnitTests.csproj --filter Category!=Integration #-v n