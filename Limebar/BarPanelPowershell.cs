/**********************************************************************************************
*   This file is part of Limebar. Limebar is copyright Xanthalas 2016 (xanthalas@live.co.uk). *
*   Limebar is free software: you can redistribute it and/or modify it under the terms of the * 
*   GNU General Public License as published by the Free Software Foundation, either version 3 *
*   of the License, or (at your option) any later version.                                    *
*                                                                                             *
*   Limebar is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;      *
*   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. *
*   See the GNU General Public License for more details.                                      *
*                                                                                             *
*   You should have received a copy of the GNU General Public License along with Limebar.     *
*   If not, see <http://www.gnu.org/licenses/>.                                               *
**********************************************************************************************/

using System;
using System.IO;
using System.Management.Automation;

namespace Limebar
{
    public class BarPanelPowershell : BarPanel,  IBarPanel
    {
        public BarPanelPowershell()
            :base()
        {
        }

        /// <summary>
        /// Update the contents of the panel
        /// </summary>
        public override void Update()
        {
            string content = string.Empty;

            string dynamicContent = runScript(this.Command, this.Options);

            var lines = dynamicContent.Split(newLine, StringSplitOptions.None);

            if (lines != null)
            {
                if (lines.Length > 0)
                {
                    dynamicContent = lines[0];

                    if (LayoutString != null && LayoutString.Length > 0)
                    {
                        content = LayoutString;
                        content = content.Replace("{content}", dynamicContent);
                    }
                    else
                    {
                        content = dynamicContent;
                    }
                }

                if (lines.Length > 1)
                {
                    TooltipText = string.Empty;

                    for (int index = 1; index < lines.Length; index++)
                    {
                        TooltipText += lines[index] += Environment.NewLine;
                    }
                }
            }

            lastUpdated = DateTime.Now;

            worker.ReportProgress(100, content);
        }

        /// <summary>
        /// Run the Powershell script and return the results
        /// </summary>
        /// <returns>A string containing the results of the script</returns>
        private string runScript(string command, string parms)
        {
            string result = string.Empty;

            if (!File.Exists(command))
            {
                result = $"Cannot find powershell script {command}";
            }
            else
            {
                PowerShell powerShell = PowerShell.Create();
                try
                {
                    powerShell.AddScript(command);
                    var results = powerShell.Invoke();

                    if (powerShell.Streams.Information != null && powerShell.Streams.Information.Count > 0)
                    {
                        string message = string.Empty;
                        foreach (var info in powerShell.Streams.Information)
                        {
                            message += info.MessageData.ToString() + Environment.NewLine;
                        }

                        result = message;
                    }
                    else
                    {
                        if (results != null && results.Count > 0)
                        {
                            result = results[0].ToString();
                        }
                    }

                }
                catch (Exception e)
                {

                    result = $"Powershell exception: {e.Message}";
                }
            }

            return result;
        }
    }
}