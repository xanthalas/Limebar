#Powershell script for use with Limebar. Return current ip address

$address = Test-Connection -ComputerName (hostname) -Count 1  | Select -ExpandProperty IPV4Address
Write-Host $address


# Example Limebar config to use this script
#
#    {
#        "ContentAlignment": 2,
#        "Foreground": "#FFFFFFFF",
#        "Background": "#FF000000",
#        "PanelType": "Powershell",
#        "Name": "Memory",
#        "DisplayOrder": 4,
#        "WidthPercent": 4,
#        "Text": "",
#        "UpdateFrequency": 2,
#        "Command": ".\\IPAddress.ps1",
#        "Options": null,
#        "ResultsFilename": null,
#        "LayoutString": "Mem: {content}",
#        "Font": "",
#        "FontSize": 0,
#        "ShowTooltip": false
#    },
