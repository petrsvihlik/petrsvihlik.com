dotnet tool restore
$OUTPUT_PATH = Join-Path $PSScriptRoot "..\Models\ContentTypes"
$appSettings = Get-Content (Join-Path $PSScriptRoot '..\appsettings.json') | Out-String | ConvertFrom-Json
dotnet tool run KontentModelGenerator -p $appSettings."DeliveryOptions"."ProjectId" -s True -o $OUTPUT_PATH -n "PetrSvihlik.Com.Models.ContentTypes"