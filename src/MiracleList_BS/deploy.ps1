cd H:\git\ML\MiracleList\src\MiracleList_BS

$Ziel = "t:\MLLPublish"
rd $Ziel -Force -Recurse -ea SilentlyContinue

$sw = [system.diagnostics.stopwatch]::StartNew()
dotnet publish -o $Ziel -c release -r win10-x64 --self-contained=true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -p:PublishTrimmed=true -p:TrimMode=link 
$sw.Stop()

$files = dir $Ziel -Recurse | ? { $_.FullName -inotmatch 'wwwroot' }
$filecount =  ($files).count
$size = [Math]::Round(($files | measure length -Sum).Sum / 1MB,2)

remove-item $Ziel/*.pdb -Force -Recurse -ea SilentlyContinue
"Published Files: $filecount/$size MB Duration: $($sw.ElapsedMilliseconds/1000)sek"
explorer $Ziel
& "T:\MLLPublish\MiracleList_BS.exe"