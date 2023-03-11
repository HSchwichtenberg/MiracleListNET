$target = "$psscriptroot\publish"
cd $psscriptroot
dotnet publish -o $target
# Die Webdriver müssen ins Ausgabeverzeichnis!
copy-item $psscriptroot\bin\Debug\netcoreapp2.0\*.exe $target -Verbose
cd $target
dotnet vstest MiracleListClientSeleniumTestsCore.dll
