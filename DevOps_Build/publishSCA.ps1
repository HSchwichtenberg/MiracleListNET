$ErrorActionPreference = "stop"
$ip = "192.168.1.191"
$no = 10
$Ziel = "\\$ip\Doc\" # !!! Vorher authentifizieren

if (-not (test-path $ziel)) { Write-Warning "Ziel nicht gefunden!" ; return }

cd $PSScriptRoot\src\Miraclelist
#dotnet publish -h
$temp = "t:\mlb$($no)_sca"
# Self-contained deployment (SCD) / Self-contained application (SCA)
dotnet publish -c release --runtime ubuntu.16.04-x64 --self-contained --framework netcoreapp2.1 -o $temp

#SCA kopieren in VM
robocopy $temp "$Ziel\MLB($no)_SCA" /e
#"dotnet miraclelist_webapi.dll --hosturl:$($ip):5000" | Set-Content "$Ziel\MLB2_SCA\miraclelist.sh"
# Framework-dependent deployments (FDD) / Portable applications (PA) 
$temp = "t:\mlb$($no)_PA"
dotnet publish -c release  --framework netcoreapp2.1 -o $temp


#PA kopieren in VM
robocopy $temp "$Ziel\MLB$($no)_PA" /e


#Quellcode auch kopieren in VM
robocopy $PSScriptRoot\ $Ziel\MLB_Source /e /XD "Packages" /xd ".vs"

Write-Host "FERTIG!" -ForegroundColor Green

