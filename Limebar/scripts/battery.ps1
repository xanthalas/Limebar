#Powershell script for use with Limebar. Return battery charge percentage.
$charge = (Get-WmiObject win32_battery).estimatedChargeRemaining

Function Check-BatteryState {
param($Laptop=$env:computername)
$Bstatus = (Get-WmiObject -Class Win32_Battery -ea 0).BatteryStatus
    if($Bstatus) {
        switch ($Bstatus)
        {
        1 { "Battery is discharging" }
        2 { "Running on AC power" }
        3 { "Fully Charged" }
        4 { "Low" }
        5 { "Critical" }
        6 { "Charging" }
        7 { "Charging and High" }
        8 { "Charging and Low" }
        9 { "Charging and Critical " }
        10 { "Unknown State" }
        11 { "Partially Charged" }            

        }
    }
}

$status = Check-BatteryState

    Write-Host $charge
    Write-Host $status

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
