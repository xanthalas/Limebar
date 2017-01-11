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
using System.ComponentModel;
using System.Windows.Media;
using System.IO;
using System.Management.Automation;

namespace Limebar
{
    public class BarPanel
    {
        #region Private members/properties
        /// <summary>
        /// Holds the last time this panel's contents were updated
        /// </summary>
        private DateTime lastUpdated = new DateTime(0);

        /// <summary>
        /// The background worker which updates this panel
        /// </summary>
        private BackgroundWorker worker { get; set; }

        /// <summary>
        /// Timer used to update the panel at the requested interval
        /// </summary>
        private System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        /// <summary>
        /// Array containing newline for splitting up result strings
        /// </summary>
        private readonly string[] newLine = { Environment.NewLine };

        #endregion

        #region Public members/properties
        /// <summary>
        /// The type of panel this is
        /// </summary>
        public string PanelType { get; set; }

        /// <summary>
        /// Possible content alignments
        /// </summary>
        public enum Alignment { Left, Right, Centre };

        /// <summary>
        /// The name of this panel
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Left to write display order for the panels
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// The width of this panel, expressed as a percentage of the whole bar
        /// </summary>
        public int WidthPercent { get; set; }

        /// <summary>
        /// The text to display in the panel
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The text to display on a tooltip over the panel
        /// </summary>
        public string TooltipText { get; set; }

        /// <summary>
        /// How to align the content
        /// </summary>
        public Alignment ContentAlignment = BarPanel.Alignment.Left;

        /// <summary>
        /// The foreground colour of this panel
        /// </summary>
        public Brush Foreground = Brushes.Green;

        /// <summary>
        /// The background colour of this panel
        /// </summary>
        public Brush Background = Brushes.Black;

        /// <summary>
        /// How often the panel should update
        /// </summary>
        public int UpdateFrequency { get; set; }

        /// <summary>
        /// The command to run
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Options which help control the contents of the panel
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// The name of the file which will contain the text to be shown in the panel
        /// </summary>
        public string ResultsFilename { get; set; }

        /// <summary>
        /// Configuration string used to format the contents.
        /// <note>Put {%content} where you want the dynamic content to be inserted</note>
        /// </summary>
        public string LayoutString { get; set; }

        /// <summary>
        /// The Font Family for the panel. Leave blank for system default.
        /// </summary>
        public string Font { get; set; }

        /// <summary>
        /// Whether to show a tooltip with additional data when the user hovers over the panel
        /// </summary>
        public bool ShowTooltip { get; set; }


        /// <summary>
        /// The Font Size for the panel. Set to -1 for system default.
        /// </summary>
        public int FontSize { get; set; }

        #endregion

        #region Public methods
        public BarPanel()
        {
            PanelType = this.GetType().ToString();

            UpdateFrequency = 0;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
        }

        /// <summary>
        /// Update the contents of this panel
        /// </summary>
        public void Update()
        {
            string content = string.Empty;
            string dynamicContent;
            switch (PanelType)
            {
                case "Clock":
                    dynamicContent = getClockText();
                    break;

                case "Powershell":
                    dynamicContent = runScript(this.Command, this.Options);
                    break;

                case "Command":
                    dynamicContent = runCommand(this.Command, this.Options);
                    break;

                case "Text":
                    dynamicContent = Text;
                    break;

                default:
                    dynamicContent = "Invalid panel type. Must be one of: Clock, Powershell, Command";
                    break;
            }
            

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
        /// Start the update process for this panel
        /// </summary>
        public void Start()
        {
            this.worker.DoWork += Worker_DoWork;
            this.worker.ProgressChanged += Worker_ProgressChanged;

            //Run the task once before putting it on the timer. Otherwise if the timer is a long one it might not get run for a while and the panel will just be blank.
            runTask();

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, this.UpdateFrequency);
            timer.Start();
        }

        /// <summary>
        /// Stop updating the panel
        /// </summary>
        public void Stop()
        {
            timer.Stop();
        }
        #endregion

        #region private methods

        /// <summary>
        /// Run the asynchronous task each time the timer ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            runTask();
        }

        private void runTask()
        {
            //If it isn't already running then run the task
            if (!this.worker.IsBusy)
            {
                this.worker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Method which does the work on a separate thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Update();
        }



        /// <summary>
        /// Updates the Text of the panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Console.WriteLine($"Setting Text to {e.UserState as string}");
            Text = e.UserState as String;
        }


        /// <summary>
        /// Retrieve the current time, format it as requested and return the results as a string
        /// </summary>
        /// <returns>A string containing the current time</returns>
        private string getClockText()
        {
            var returnString = string.Empty;

            if (Options.Contains("ShowDate"))
            {
                returnString += DateTime.Now.ToShortDateString() + " ";
            }

            if (Options.Contains("ShowSeconds"))
            {
                returnString += DateTime.Now.ToLongTimeString();
            }
            else
            {
                returnString += DateTime.Now.ToShortTimeString();
            }

            return returnString;
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


        /// <summary>
        /// Run the Powershell script and return the results as a string
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

        #endregion
    }
}
