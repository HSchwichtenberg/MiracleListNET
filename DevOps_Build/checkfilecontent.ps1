#cd $PSScriptRoot

$datei = get-content "src\Miraclelist\wwwroot\AddedColumnsConfig.txt"

foreach($z in $datei)
{
if  (-not($z.StartsWith("#"))) { Write-Error "Additional Columns müssen auskommentiert sein: $z" }
else { "OK: $z" }
}


return 0

'B:\A2\_work\_temp\src\Miraclelist\wwwroot\AddedColumnsConfig.txt'