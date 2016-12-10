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

namespace Limebar
{
    public class BarPanelCommand : BarPanel, IBarPanel
    {
        public BarPanelCommand()
            :base()
        {
        }

        /// <summary>
        /// Update the contents of the panel
        /// </summary>
        public override void Update()
        {
            string content = string.Empty;

            string dynamicContent = runCommand(this.Command, this.Options);
            dynamicContent = dynamicContent.TrimEnd('\r', '\n');
            if (LayoutString != null && LayoutString.Length > 0)
            {
                content = LayoutString;
                content = content.Replace("{content}", dynamicContent);
            }
            else
            {
                content = dynamicContent;
            }

            lastUpdated = DateTime.Now;

            worker.ReportProgress(100, content);

        }

        /// <summary>
        /// Run the command and return the results as a string
        /// </summary>
        /// <returns>A string containing the output from the command</returns>
        private string runCommand(string command, string parms)
        {
            string output = string.Empty;

            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(command);
                psi.Arguments = parms;
                psi.RedirectStandardOutput = true;
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                System.Diagnostics.Process proc;
                proc = System.Diagnostics.Process.Start(psi);
                output = proc.StandardOutput.ReadToEnd();

            }
            catch (Exception e)
            {
                output = $"Command failed: {e.Message}";
            }

            return output;
        }
    }
}
