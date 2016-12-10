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
    class BarPanelClock : BarPanel, IBarPanel
    {
        /// <summary>
        /// Create a new BarPanelClock
        /// </summary>
        /// <param name="backgroundWorker"></param>
        public BarPanelClock()
            :base()
        {
        }

        /// <summary>
        /// Update the contents of the panel
        /// </summary>
        public override void Update()
        {
            string content = string.Empty;

            string dynamicContent = getClockText();

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
        /// Retrieve the current time and format it as requested
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
    }
}