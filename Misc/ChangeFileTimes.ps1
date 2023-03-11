$files = Get-ChildItem H:\ML\MiracleListBackend_TFS\src\MiracleList_CSB\wwwroot\app-icon -force -File

$date = Get-Date
foreach($file in $files)
{
$file.fullname
$file.CreationTime=$date
$file.LastAccessTime=$date
$file.LastWritetime=$date
}