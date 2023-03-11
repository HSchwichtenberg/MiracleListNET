#Verweis auf das lokale Unterzeichnis ProjectPackages
#muss in DevOps-Pipeline ersetzt werden durch ..\s\ProjectPackages-->

$file = "nuget.config"
$file
"--- Vorher:"
$c
$c = get-content $file -Raw
$c = $c.Replace("ProjectPackages","..\s\ProjectPackages")
$c | set-Content $file
"--- Nachher:"
$c