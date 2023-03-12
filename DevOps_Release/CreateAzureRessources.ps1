# Azure-Ressourcen für Release-Pipeline mit PowerShell anlegen
# (C) Dr. Holger Schwichtenberg 2018
# Verwendet noch das alte Azure-PowerShell-Modul 

$ErrorActionPreference = "stop"

#region Installation
Install-Module PowerShellGet -Force -Scope currentuser # globaler Scope geht nur als Admin!
Install-Module -Name AzureRM -force -AllowClobber -Scope currentuser
Import-Module -Name AzureRM 
Get-Module AzureRM -ListAvailable | Select-Object -Property Name,Version,Path | ft
"Anzahl geladener Azure-Module: " + (Get-Module AzureRM.* -ListAvailable | Select-Object -Property Name,Version,Path).Count
"Anzahl verfügbarer Azure-Befehle: " + (Get-Command -module AzureRM*).Count
#endregion

# Anmelden interaktiv!!!
Login-AzureRmAccount 

# Wichtige Festlegungen
$prefix = "HD-"
$RessourceGroup = "DEMO_MiracleList"
$Serviceplan = "DEMOMiracleListFREE"
$location="West Europe"

#region Ressourcen prüfen
# Optional: Create a resource group.
#New-AzureRmResourceGroup -Name myResourceGroup -Location $location
#Remove-AzureRmResourceGroup -Name myResourceGroup 
$rg = Get-AzureRmResourceGroup $RessourceGroup
if (-not $sp) { Write-Error "Resource Group nicht gefunden!" : return }
# Optional: Service Plan anlegen
#New-AzureRmAppServicePlan -Name $webappname -Location $location -ResourceGroupName myResourceGroup -Tier Free
$sp = Get-AzureRmAppServicePlan -Name $Serviceplan
if (-not $sp) { Write-Error "Service Plan nicht gefunden!" : return }

# TODO:
#$lo = Get-AzureLocation $location
#if (-not $lo) { Write-Error "Location nicht gefunden!" : return }
#endregion

#region Websites
$webappnames="$($prefix)MLBackend-Produktion","$($prefix)MLBackend-Staging","$($prefix)MLClient-Produktion","$($prefix)MLClient-Staging"

foreach($webappname in $webappnames)
{
Write-Host "Creating $webappname" -ForegroundColor Yellow
# Create a web app.
New-AzureRmWebApp -Name $webappname -Location $location -AppServicePlan $Serviceplan -ResourceGroupName $RessourceGroup
$wa = Get-AzureRmWebApp -Name $webappname
if ($wa -eq $null) { Write-Error "WebApp wurde nicht angelegt!"; return; }
else { write-host "OK" -ForegroundColor Green }
}
# Kontrolle:
Get-AzureRmWebApp -ResourceGroupName $RessourceGroup | where RepositorySiteName -like "*$prefix*" | ft

# Get publishing profile for the web app
#$xml = (Get-AzureRmWebAppPublishingProfile -Name $webappname `
#-ResourceGroupName DEMO_MiracleList `
#-OutputFile null)
#endregion

#region SQL Server
$dbnames="$($prefix)MLDB-Staging", "$($prefix)MLDB-Produktion"

# The logical server name: Use a random value or replace with your own value (do not capitalize)
$servername = "$($prefix)MLSQLServer"
# Set an admin login and password for your database
# The login information for the server
$adminlogin = "MLSQLAdmin"
$password = New-Guid
Write-Host "SQL Server password is: $password" -ForegroundColor Green
# The ip address range that you want to allow to access your server - change as appropriate
$startip = "0.0.0.0"
$endip = "255.255.255.255"
# The database name

New-AzureRmSqlServer -ResourceGroupName $RessourceGroup `
    -ServerName $servername `
    -Location $location `
    -SqlAdministratorCredentials $(New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $adminlogin, $(ConvertTo-SecureString -String $password -AsPlainText -Force))

New-AzureRmSqlServerFirewallRule -ResourceGroupName $RessourceGroup `
    -ServerName $servername `
    -FirewallRuleName "AllowAll" -StartIpAddress "0.0.0.0" -EndIpAddress $endip

foreach($databasename in $dbnames)
{ 
New-AzureRmSqlDatabase  -ResourceGroupName $RessourceGroup -ServerName $servername  -DatabaseName $databasename    -RequestedServiceObjectiveName "S0"
}

# Kontrolle
Get-AzureRmSqlDatabase  -ResourceGroupName $resourcegroupname -ServerName $servername  | ft

#endregion

#region Alles wieder löschen (mit Nachfrage!)
$webappnames | % { Remove-AzureRmWebApp -name $_ -ResourceGroupName $RessourceGroup -confirm }
Remove-AzureRmSqlServer -ResourceGroupName $resourcegroupname -ServerName $servername -Confirm
#endregion