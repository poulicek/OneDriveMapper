using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace OneDriveMapper
{
    static class Program
    {
        private const string REG_NAME = "OneDriveMapping";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            processArgs(args);
        }


        private static void processArgs(string[] args)
        {
            var silent = args.Length == 1 && args[0] == "-s";
            var settings = Settings.Load("OneDriveMapper.config");

            applySettings(settings, !silent);
        }

        #region UI

        /// <summary>
        /// Applies the settings
        /// </summary>
        private static void applySettings(Settings settings)
        {
            setStartup(settings.AutoRun);
            mapDrive(settings.Drive, settings.CID, settings.Username, settings.Password);
        }


        /// <summary>
        /// Shows the UI
        /// </summary>
        private static void applySettings(Settings settings, bool showUI)
        {
            if (!showUI)
                applySettings(settings);
            else
            {
                while (!showSettings(settings))
                    continue;
            }   
        }


        /// <summary>
        /// Shows the settings form
        /// </summary>
        private static bool showSettings(Settings settings)
        {
            var result = true;
            using (var dlg = new SettingsForm(settings))
            {
                dlg.FormClosing += (s, e) =>
                {
                    try
                    {
                        if (dlg.DialogResult == DialogResult.OK)
                        {
                            settings.Save();
                            applySettings(settings);
                            MessageBox.Show("Mapping finished!", "OneDrive Mapper", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "OneDrive Mapper - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        result = false;
                    }
                };
                dlg.ShowDialog();
            }

            return result;
        }


        #endregion

        #region System Calls

        /// <summary>
        /// Maps the drive
        /// </summary>
        private static void mapDrive(char drive, string cid, string username, string password)
        {
            ProcessAsUser.Launch($"net.exe use {drive.ToString().ToUpper()}: \"https://d.docs.live.net/{cid}\" /user:{username} {password} /persistent:no");


            //using (var p = new Process())
            //{
            //    p.StartInfo = new ProcessStartInfo()
            //    {
            //        WindowStyle = ProcessWindowStyle.Hidden,
            //        CreateNoWindow = true,
            //        FileName = "net.exe",
            //        Arguments = $"use {drive.ToString().ToUpper()}: \"https://d.docs.live.net/{cid}\" /user:{username} {password} /persistent:no",
            //        UseShellExecute = false,
            //        RedirectStandardOutput = true,
            //        RedirectStandardError = true,
            //    };
            //    p.Start();

            //    var error = p.StandardError.ReadToEnd();
            //    p.WaitForExit();

            //    if (!string.IsNullOrEmpty(error))
            //        throw new Exception(error);
            //}
        }

        /// <summary>
        /// Setting the startup state
        /// </summary>
        private static bool setStartup(bool set)
        {
            try
            {
                var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (set)
                    rk.SetValue(REG_NAME, Application.ExecutablePath.ToString() + " -s");
                else
                    rk.DeleteValue(REG_NAME, false);

                return set;
            }
            catch { return !set; }
        }
    }

    #endregion
}
