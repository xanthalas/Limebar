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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfAppBar;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;

namespace Limebar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private members

        private Settings settings = new Settings();

        private Window invisibleOwnerWindow;

        private const string CONFIG_FILE = "config.json";

        private string configuration_file;

        private System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        private List<BarPanel> panels = new List<BarPanel>();
        private List<BackgroundWorker> workers = new List<BackgroundWorker>();
        private DateTime configFileLastUpdated = new DateTime(0);

        private bool rebuildPanels = true;

        #endregion

        #region Public methods
        public MainWindow()
        {

            string[] args = Environment.GetCommandLineArgs();

            if (args != null && args.Length > 1)
            {
                configuration_file = args[1];
            }
            else
            {
                configuration_file = CONFIG_FILE;
            }
            InitializeComponent();
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Create an invisible window which will own the main one. This is needed to prevent Limebar showing up in the Alt-Tab list.
        /// </summary>
        /// <returns>Window object </returns>
        private Window createInvisibleOwnerWindow()
        {
            Window w = new Window(); // Create helper window
            w.Top = -100; // Location of new window is outside of visible part of screen
            w.Left = -100;
            w.Width = 1; // size of window is enough small to avoid its appearance at the beginning
            w.Height = 1;
            w.WindowStyle = WindowStyle.ToolWindow; // Set window style as ToolWindow to avoid its icon in AltTab 
            w.ShowInTaskbar = false;
            w.Show(); // We need to show window before set is as owner to our main window
            w.Hide(); // Hide helper window just in case
            return w;
        }

        /// <summary>
        /// Create the bar
        /// </summary>
        private void createBar()
        {
            this.Height = settings.BarHeight;
            this.VerticalAlignment = settings.PanelAlignment;
            this.Background = Brushes.Black;
            this.Foreground = Brushes.Aquamarine;
            if (settings.BarLocation == BarPosition.Top)
            {
                AppBarFunctions.SetAppBar(this, ABEdge.Top, mainPanel);
            }
            else
            {
                AppBarFunctions.SetAppBar(this, ABEdge.Bottom, mainPanel);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            checkIfConfigFileUpdated();
            updateAllPanels();
        }

        private void checkIfConfigFileUpdated()
        {
            if (File.Exists(configuration_file))
            {
                var fileInfo = new FileInfo(configuration_file);
                if (fileInfo.LastWriteTime != configFileLastUpdated)
                {
                    //Stop all the timers
                    panels.ForEach(p => p.Stop());

                    rebuildPanels = true;
                    loadConfig(configuration_file);

                    panels.ForEach(p => p.Start());
                }
            }
            else
            {
                //If it doesn't exist now then the load routine will detect that and put the appropriate message up
                loadConfig(configuration_file);
                configFileLastUpdated = new DateTime(0);
            }

        }


        private void updateAllPanels()
        {
            if (rebuildPanels)
            {
                mainPanel.Children.Clear();

                var orderedPanels = panels.OrderBy(o => o.DisplayOrder);

                foreach (var panel in orderedPanels)
                {
                    var newLabel = new Label();
                    newLabel.Name = panel.Name;
                    newLabel.VerticalAlignment = settings.PanelAlignment;

                    if (panel.Font != null && panel.Font.Length > 0)
                    {
                        //Try and create the font from the string. If it doesn't exist then no error is thrown, it just appears to set it to the default
                        var fontFamily = new FontFamily(panel.Font);

                        if (fontFamily != null)
                        {
                            newLabel.FontFamily = fontFamily;
                        }
                    }
                    if (panel.FontSize > 0)
                    {
                        newLabel.FontSize = panel.FontSize;
                    }

                    newLabel.Content = panel.Text;

                    if (panel.WidthPercent > 0)
                    {
                        newLabel.Width = Math.Round((double)System.Windows.SystemParameters.PrimaryScreenWidth * ((double)panel.WidthPercent / 100d));
                    }
                    newLabel.Foreground = panel.Foreground;
                    newLabel.Background = panel.Background;

                    switch (panel.ContentAlignment)
                    {
                        case BarPanel.Alignment.Left:
                            newLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
                            break;
                        case BarPanel.Alignment.Right:
                            newLabel.HorizontalContentAlignment = HorizontalAlignment.Right;
                            break;
                        case BarPanel.Alignment.Centre:
                            newLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                            break;
                    }
                    var margin = newLabel.Margin;
                    newLabel.Margin = new Thickness(0);
                    newLabel.Padding = new Thickness(0);

                    if (panel.ShowTooltip)
                    {
                        ToolTip tooltip = new ToolTip();
                        tooltip.Content = removeTrailingBlankLines(panel.TooltipText);
                        newLabel.ToolTip = tooltip;
                    }

                    mainPanel.Children.Add(newLabel);

                }

                rebuildPanels = false;
            }
            else
            {
                foreach (Label label in mainPanel.Children)
                {
                    var panel = panels.Where(p => p.Name == label.Name).First();

                    if (panel != null)
                    {
                        label.Content = panel.Text;

                        if (panel.ShowTooltip)
                        {
                            var tooltip = label.ToolTip as ToolTip;
                            tooltip.Content = removeTrailingBlankLines(panel.TooltipText);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove blank lines from the end of the string but keep those embedded within
        /// </summary>
        /// <param name="input">String to remove lines from</param>
        /// <returns>String with trailing blank likes remove</returns>
        private string removeTrailingBlankLines(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            string result = string.Empty;

            bool foundNonBlankLine = false;

            var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int index = lines.Length - 1; index >= 0; index --)
            {
                if (lines[index].Trim().Length > 0)
                {
                    result = lines[index] += Environment.NewLine + result;
                    foundNonBlankLine = true;
                }
                else
                {
                    if (foundNonBlankLine)
                    {
                        result = lines[index] += Environment.NewLine + result;
                    }
                }
            }

            return result.TrimEnd();
        }

        /// <summary>
        /// Load the config file
        /// </summary>
        /// <param name="file"></param>
        private void loadConfig(string file)
        {
            settings = new Settings();

            if (!File.Exists(configuration_file))
            {
                createErrorPanel($"Cannot find configuration file {file}");
                return;
            }

            try
            {
                string jsonContents = File.ReadAllText(file);
                settings = JsonConvert.DeserializeObject<Settings>(jsonContents);
                loadPanels(settings.Panels);
                var fileInfo = new FileInfo(file);
                configFileLastUpdated = fileInfo.LastWriteTime;
            }
            catch (Exception e)
            {
                createErrorPanel(e.Message);

                if (File.Exists(configuration_file))
                {
                    var fileInfo = new FileInfo(file);
                    configFileLastUpdated = fileInfo.LastWriteTime;
                }
                else
                {
                    configFileLastUpdated = new DateTime(0);
                }
            }
        }

        /// <summary>
        /// Create one big error panel to show the error encountered during config file loading
        /// </summary>
        /// <param name="message">The message to show</param>
        private void createErrorPanel(string message)
        {
            panels.Clear();
            BarPanel errorPanel = new BarPanel();
            errorPanel.WidthPercent = 100;
            errorPanel.Text = "Error loading config file: " + message;
            errorPanel.Background = Brushes.Red;
            errorPanel.Foreground = Brushes.Black;
            panels.Add(errorPanel);
        }

        /// <summary>
        /// Load the panels into the bar
        /// </summary>
        /// <param name="loadedPanels">The collection of panels as loaded from the config file</param>
        private void loadPanels(List<BarPanel> loadedPanels)
        {
            panels.Clear();

            loadedPanels.ForEach(p => panels.Add(p));
        }

        #endregion

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadConfig(configuration_file);
            createBar();

            updateAllPanels();

            invisibleOwnerWindow = createInvisibleOwnerWindow();
            this.Owner = invisibleOwnerWindow;

            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);

            this.WindowStyle = WindowStyle.None;

            timer.Start();
            panels.ForEach(p => p.Start());
        }

        /// <summary>
        /// Kill the invisible owner window to ensure it all shuts down neatly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            panels.ForEach(p => p.Stop());

            this.Owner = null;
            invisibleOwnerWindow.Close();
            invisibleOwnerWindow = null;
        }
        #endregion
    }
}
