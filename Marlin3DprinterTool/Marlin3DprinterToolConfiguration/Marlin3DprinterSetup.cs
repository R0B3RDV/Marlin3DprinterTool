﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using PayPal;


namespace Marlin3DprinterToolConfiguration
{
    public partial class Marlin3DprinterSetup : Form
    {
        private readonly Configuration _configuration = new Configuration();

        public Marlin3DprinterSetup()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            _configuration.LicenseKey = txtBxUnlockKey.Text;
            Close();
        }


        private void ShowLicense()
        {
            if (String.IsNullOrEmpty(txtBxUnlockKey.Text)) return;

            txtBxShowLicence.Text = Configuration.Decrypt(txtBxUnlockKey.Text).Replace(";", Environment.NewLine);
        }

        private void txtBxUnlockKey_TextChanged(object sender, EventArgs e)
        {
            ShowLicense();
        }

        private void btnPayPal_Click(object sender, EventArgs e)
        {
            Donation paypal = new Donation();

            paypal.Donatebutton();
        }

        








        private void btnDirectoryCurrentFirmware_Click(object sender, EventArgs e)
        {
            Configuration configuration = new Configuration();
            FolderBrowserDialog currentFirmwareBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = configuration.CurrentFirmware,
                Description = @"Directory to current Firmware"
            };
            DialogResult result = currentFirmwareBrowserDialog.ShowDialog();
            if (result == DialogResult.OK) txtBxDirectoryCurrentFirmware.Text = currentFirmwareBrowserDialog.SelectedPath;
        }

        private void btnDirectoryNewFirmware_Click(object sender, EventArgs e)
        {
            Configuration configuration = new Configuration();
            FolderBrowserDialog newFirmwareBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = configuration.NewFirmware,
                Description = @"Directory to new Firmware"
            };
            DialogResult result = newFirmwareBrowserDialog.ShowDialog();
            if (result == DialogResult.OK) txtBxDirectoryNewFirmware.Text = newFirmwareBrowserDialog.SelectedPath;
        }

        private void btnArduinoIDE_Click(object sender, EventArgs e)
        {
            Configuration configuration = new Configuration();
            FolderBrowserDialog arduinoIdeBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = configuration.ArduinoIde,
                Description = @"Directory to Arduino IDE"
            };
            DialogResult result = arduinoIdeBrowserDialog.ShowDialog();
            if (result == DialogResult.OK) txtBxArduinoIDE.Text = arduinoIdeBrowserDialog.SelectedPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            _configuration.CurrentFirmware = txtBxDirectoryCurrentFirmware.Text;
            _configuration.NewFirmware = txtBxDirectoryNewFirmware.Text;
            _configuration.ArduinoIde = txtBxArduinoIDE.Text;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Marlin3DprinterSetup_Load(object sender, EventArgs e)
        {
            Configuration configuration = new Configuration();
            txtBxDirectoryCurrentFirmware.Text = configuration.CurrentFirmware;
            txtBxDirectoryNewFirmware.Text = configuration.NewFirmware;
            txtBxArduinoIDE.Text = configuration.ArduinoIde;

            txtBxUnlockKey.Text = configuration.LicenseKey;
            ShowLicense();

        }

    }

   
    /// <summary>
    /// 
    /// </summary>
    public static class FileAssociation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="progID"></param>
        /// <param name="description"></param>
        /// <param name="icon"></param>
        /// <param name="application"></param>
        // Associate file extension with progID, description, icon and application
        public static void Associate(string extension,
            string progID, string description, string icon, string application)
        {
            RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(extension);
            registryKey?.SetValue("", progID);


            if (string.IsNullOrEmpty(progID)) return;
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progID))
            {
                if (description != null)
                {
                    key?.SetValue("", description);
                }
                if (icon != null)
                {
                    var subKey = key?.CreateSubKey("DefaultIcon");
                    subKey?.SetValue("", ToShortPathName(icon));
                }
                if (application != null)
                {
                    var subKey = key?.CreateSubKey(@"Shell\Open\Command");
                    subKey?.SetValue("", ToShortPathName(application) + " \"%1\"");
                }
            }
            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }


        [DllImport("Kernel32.dll")]
        private static extern uint GetShortPathName(string lpszLongPath,
            [Out] StringBuilder lpszShortPath, uint cchBuffer);

        // Return short path format of a file name
        private static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            GetShortPathName(longName, s, iSize);
            return s.ToString();
        }

        //    // Tell explorer the file association has been changed
        //    SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}