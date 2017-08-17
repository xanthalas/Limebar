
#Powershell script for use with Limebar. Return now playing and playlist from Bard

$nowplaying = c:\tools\bard\bard.exe nowplaying
Write-Host $nowplaying

$result = c:\tools\bard\bard.exe playlist
$first = 1
foreach ($line in $result)
{
     if ($first -eq 0)              #Strip off the first line
     {
        Write-Host $line
     }
     $first = 0
}



# Example Limebar config to use this script
#
#   {
#       "ContentAlignment": 0,
#       "Foreground": "#FFA52A2A",
#       "Background": "#FF000000",
#       "PanelType": "Powershell",
#       "Name": "Bard",
#       "DisplayOrder": 1,
#       "WidthPercent": 35,
#       "Text": "Waiting...",
#       "UpdateFrequency": 10,
#       "Command": ".\\music.ps1",
#       "Options": "NowPlaying",
#       "ResultsFilename": null,
#       "LayoutString": "",
#       "Font":"",
#       "FontSize": 0,
#       "ShowTooltip": true
#   },
