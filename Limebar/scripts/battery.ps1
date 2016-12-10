#Powershell script for use with Limebar. Return battery charge percentage.
(Get-WmiObject win32_battery).estimatedChargeRemaining

# Example Limebar config to use this script
#
#    {
#        "ContentAlignment": 1,
#        "Foreground": "#FFFFFFFF",
#        "Background": "#FF000000",
#        "PanelType": "Limebar.BarPanelPowershell",
#        "Name": null,
#        "DisplayOrder": 5,
#        "WidthPercent": 3,
#        "Text": "",
#        "UpdateFrequency": 30,
#        "Command": ".\\battery.ps1",
#        "Options": null,
#        "ResultsFilename": null,
#        "LayoutString": "Batt: {%content}% "
#    },
