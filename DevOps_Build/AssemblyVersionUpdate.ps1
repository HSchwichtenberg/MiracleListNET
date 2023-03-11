Write-host "Set the Version Numbers in all .NET Core .csproj files" -foregroundcolor yellow
Write-host "Dr. Holger Schwichtenberg, www.IT-Visions.de 2018-2022" -foregroundcolor yellow
Write-host "------------------------------------------------------" -foregroundcolor yellow

$CurrentVersion = "2.14.0"
$defaultBuildId = 0
$ErrorActionPreference = "stop"
#region Constructing parameters
$buildid = $env:Build_BuildId
if (-not($buildid)) { $buildid = $defaultBuildId } # for lokal Test
$version = $env:version
if (-not($version)) { $version = $CurrentVersion } # for lokal Test
$version = "$version" + "." + $buildid
$sourcesDirectory = $env:BUILD_SOURCESDIRECTORY
if (-not($sourcesDirectory)) { $sourcesDirectory = (Get-item $PSScriptRoot).Parent.FullName } # for lokal Test
$sourcesDirectory
cd $sourcesDirectory
#endregion

Function Set-NodeValue($rootNode, [string]$nodeName, [string]$value)
{   
    $nodePath = "PropertyGroup/$($nodeName)"  
    $node = $rootNode.Node.SelectSingleNode($nodePath)
    if ($node -eq $null) {
        Write-Host "  Add <$($nodeName)>"

        $group = $rootNode.Node.SelectSingleNode("PropertyGroup")
        $node = $group.OwnerDocument.CreateElement($nodeName)
        $group.AppendChild($node) | Out-Null
    }
    $node.InnerText = $value
    Write-Host "  Set <$($nodeName)> = $($value)"
}

Write-Host "---> Searching for *.csproj in $($sourcesDirectory)"
$count = 0
Get-ChildItem -Path $sourcesDirectory -Filter "*.csproj" -Recurse -File -Exclude "ITVisions.Blazor.csproj" | 
    ForEach-Object { 
        $count++
        Write-Host "- Project #$($count):" $($_.FullName)

        $projectPath = $_.FullName
        $project = Select-Xml $projectPath -XPath "//Project"
        if (-not($project)) { Write-Warning "Not a valid .NET Core project" ; return }
        Set-NodeValue $project "Version" $version
        Set-NodeValue $project "AssemblyVersion" $version
        Set-NodeValue $project "FileVersion" $version
        Set-NodeValue $project "InformationalVersion" "$version-$(Get-Date)" 
        Set-NodeValue $project "Copyright" "Dr. Holger Schwichtenberg, www.IT-Visions.de 2018-$((Get-Date).Year)"

        $document = $project.Node.OwnerDocument
        $document.PreserveWhitespace = $true

        $document.Save($projectPath)

        Write-Host ""
    }

Write-Host "##vso[build.updatebuildnumber]$($version)"