#Powershell script for use with Limebar. Return processor usage percentage.

$RESULT = (Get-WmiObject win32_processor) | Measure-Object -Property LoadPercentage -Average | Select Average

Write-Host $RESULT.Average


# Example Limebar config to use this script
#
#    {
#        "ContentAlignment": 1,
#        "Foreground": "#FFFFFFFF",
#        "Background": "#FF000000",
#        "PanelType": "Limebar.BarPanelPowershell",
#        "Name": null,
#        "DisplayOrder": 3,
#        "WidthPercent": 4,
#        "Text": "",
#        "UpdateFrequency": 2,
#        "Command": ".\\processor.ps1",
#        "Options": null,
#        "ResultsFilename": null,
#        "LayoutString": "CPU: {%content}%"
#    },
