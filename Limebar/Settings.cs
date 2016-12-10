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
    public enum BarPosition { Top, Bottom };
    public class Settings
    {
        /// <summary>
        /// The height of the bar
        /// </summary>
        public int BarHeight { get; set; }

        /// <summary>
        /// The location of the bar on the screen
        /// </summary>
        public BarPosition BarLocation { get; set; }

        /// <summary>
        /// How panels (and the text within them) should be aligned with the bar
        /// </summary>
        public VerticalAlignment PanelAlignment { get; set; }

        /// <summary>
        /// The panels to show on the bar
        /// </summary>
        public List<BarPanel> Panels { get; set; }
    }
}
