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

namespace Limebar
{
    public class BarPanel
    {
        /// <summary>
        /// Holds the last time this panel's contents were updated
        /// </summary>
        protected DateTime lastUpdated = new DateTime(0);

        /// <summary>
        /// The background worker which updates this panel
        /// </summary>
        protected BackgroundWorker worker { get; set; }

        /// <summary>
        /// Timer used to update the panel at the requested interval
        /// </summary>
        protected System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        /// <summary>
        /// Array containing newline for splitting up result strings
        /// </summary>
        protected readonly string[] newLine = { Environment.NewLine };

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
        public virtual void Update()
        {

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


        /// <summary>
        /// Run the asynchronous task each time the timer ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer_Tick(object sender, EventArgs e)
        {
            runTask();
        }

        protected void runTask()
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
        protected void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Update();
        }



        /// <summary>
        /// Updates the Text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Console.WriteLine($"Setting Text to {e.UserState as string}");
            Text = e.UserState as String;
        }
    }
}
