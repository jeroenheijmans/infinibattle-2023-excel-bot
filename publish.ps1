$csprojFolder = "./ExcelBot/"

# Globals and script vars
$ErrorActionPreference = "Stop"
$APIKEY = Get-Content apiKey
$BASEURL = "https://infinibattle.infi.nl"
$CONFIGURATION = "Release"
$binDirectory = "$($csprojFolder)\bin\$($CONFIGURATION)\net6.0"
$zipPath = "$($PSScriptRoot)\publish.zip"

# Build the bot
dotnet build --configuration $CONFIGURATION

# Remove old zip file, if any
if (Test-Path $zipPath) { Remove-Item ($zipPath) }

# Compress DLLs to zip file
Get-ChildItem -Path $binDirectory -Exclude runtimes | Compress-Archive -DestinationPath $zipPath

# Upload new zip file
$uploadUrl = "$($BASEURL)/api/uploadBot/$($APIKEY)"
$response = (New-Object Net.WebClient).UploadFile($uploadUrl, $zipPath)
[System.Text.Encoding]::UTF8.GetString($response)