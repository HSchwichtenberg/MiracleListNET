# Dateien umbenennen
dir .\_framework\_bin | rename-item -NewName { $_.name -replace ".dll\b",".bin" }
# Dateinamen in blazor.boot.json ändern
((Get-Content .\_framework\blazor.boot.json -Raw) -replace '.dll"','.bin"') | Set-Content .\_framework\blazor.boot.json
