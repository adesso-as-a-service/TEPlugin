/*
  SamplePlugin - An Example KeePass Plugin
  Copyright (C) 2003-2019 Dominik Reichl <dominik.reichl@t-online.de>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.IO;

using KeePass.Forms;
using KeePass.Plugins;
using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;

// The namespace name must be the same as the file name of the
// plugin without its extension.
// For example, if you compile a plugin 'SamplePlugin.dll',
// the namespace must be named 'SamplePlugin'.
namespace TEPlugin
{
	// Namespace name 'SamplePlugin' + 'Ext' = 'SamplePluginExt'
	public sealed class TEPluginExt : Plugin
	{
		// The plugin remembers its host in this variable
		private IPluginHost m_host = null;

        // Location of the used pkcs11Library
        private string pkcs11LibraryPath = null;

        // Name of the configuration option to store library path
        private const string OptionLibraryPath = "TEPlugin_LibraryPath";

        private TEKeyprovider keyProvider;

        /// <summary>
        /// The <c>Initialize</c> method is called by KeePass when
        /// you should initialize your plugin.
        /// </summary>
        /// <param name="host">Plugin host interface. Through this
        /// interface you can access the KeePass main window, the
        /// currently opened database, etc.</param>
        /// <returns>You must return <c>true</c> in order to signal
        /// successful initialization. If you return <c>false</c>,
        /// KeePass unloads your plugin (without calling the
        /// <c>Terminate</c> method of your plugin).</returns>
        public override bool Initialize(IPluginHost host)
		{
			if(host == null) return false; // Fail; we need the host
			m_host = host;
            
            // read Library Location, if it exists
            pkcs11LibraryPath = m_host.CustomConfig.GetString(OptionLibraryPath, null);
            if (pkcs11LibraryPath == null || pkcs11LibraryPath.Length == 0)
            {
                MessageBox.Show("To enable Threshold Encryption go to settings and select the pkcs11-library!");
            } else
            {
                keyProvider = new TEKeyprovider(pkcs11LibraryPath);
                m_host.KeyProviderPool.Add(keyProvider);
            }
            

			return true; // Initialization successful
		}

		/// <summary>
		/// The <c>Terminate</c> method is called by KeePass when
		/// you should free all resources, close files/streams,
		/// remove event handlers, etc.
		/// </summary>
		public override void Terminate()
		{
            m_host.CustomConfig.SetString(OptionLibraryPath, pkcs11LibraryPath);
            m_host.KeyProviderPool.Remove(keyProvider);
        }

		/// <summary>
		/// Get a menu item of the plugin. See
		/// https://keepass.info/help/v2_dev/plg_index.html#co_menuitem
		/// </summary>
		/// <param name="t">Type of the menu that the plugin should
		/// return an item for.</param>
		public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
		{
			// Our menu item below is intended for the main location(s),
			// not for other locations like the group or entry menus
			if(t != PluginMenuType.Main) return null;

			ToolStripMenuItem tsmi = new ToolStripMenuItem("TEPlugin");

            ToolStripMenuItem tsmiSelectPkcs11Library = new ToolStripMenuItem("Select PKCS11-Library");
            tsmi.DropDownItems.Add(tsmiSelectPkcs11Library);
            tsmiSelectPkcs11Library.Click += this.OnOptionsClicked;

            return tsmi;
		}

        private void OnOptionsClicked(object sender, EventArgs e)
        {
            // Called when the menu item is clicked
            // read Pkcs11 Library Path
        
            OpenFileDialog ofd = new OpenFileDialog();
            if (pkcs11LibraryPath == null)
            {
                ofd.InitialDirectory = "C:\\";
            } else
            {
                ofd.InitialDirectory = Path.GetDirectoryName(pkcs11LibraryPath);
            }
            
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;
            ofd.Filter = ".dll files (*.dll)|*.dll";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pkcs11LibraryPath = ofd.FileName;
                if (keyProvider != null)
                    m_host.KeyProviderPool.Remove(keyProvider);
                keyProvider = new TEKeyprovider(pkcs11LibraryPath);
                m_host.KeyProviderPool.Add(keyProvider);
            }

        }


    }
}
