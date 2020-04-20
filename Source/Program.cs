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
        private static string applySettings(Settings settings)
        {
            setStartup(settings.AutoRun);
            return mapDrive(settings.Drive, settings.Url, settings.Username, settings.Password);
        }


        /// <summary>
        /// Shows the UI
        /// </summary>
        private static void applySettings(Settings settings, bool showUI)
        {
            try
            {
                if (!showUI)
                    applySettings(settings);
                else if (showSettings(settings))
                {
                    var message = applySettings(settings);
                    MessageBox.Show(string.IsNullOrEmpty(message) ? "Mapping finished!" : message, "OneDrive Mapper", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OneDrive Mapper - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Shows the settings form
        /// </summary>
        private static bool showSettings(Settings settings)
        {
            var dlg = new SettingsForm(settings);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                settings.Save();
                return true;
            }

            return false;
        }

        #endregion

        #region System Calls

        /// <summary>
        /// Maps the drive
        /// </summary>
        private static string mapDrive(char drive, string url, string username, string password)
        {
            using (var p = new Process())
            {
                p.StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "net.exe",
                    Arguments = $"use {drive.ToString().ToUpper()}: \"{url}\" /user:{username} {password} /persistent:no",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                };
                p.Start();

                var result = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                return result;
            }
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
