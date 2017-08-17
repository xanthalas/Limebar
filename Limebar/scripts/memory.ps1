#Powershell script for use with Limebar. Return the percentage of free memory.

$os = Get-Ciminstance Win32_OperatingSystem
$pctFree = [math]::Round(($os.FreePhysicalMemory/$os.TotalVisibleMemorySize)*100)
Write-Host $pctFree 

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
#        "Command": ".\\memory.ps1",
#        "Options": null,
#        "ResultsFilename": null,
#        "LayoutString": "Mem: {content}",
#        "Font": "",
#        "FontSize": 0,
#        "ShowTooltip": false
#    },
