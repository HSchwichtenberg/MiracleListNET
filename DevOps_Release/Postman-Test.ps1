# Startet Postman Test gegen Entwicklungsrechner
cls
cd $PSScriptRoot
newman run MiracleList-APITests.postman_collection.json `
-e MiracleList-APITests.postman_environment.dev.json --disable-unicode `
--reporters cli,junit --reporter-junit-export TestResults/newman-run-report.xml