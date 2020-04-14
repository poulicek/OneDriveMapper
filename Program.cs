using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace OneDriveMapper
{
    static class Program
    {
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

            if (silent)
            {
                applySettings(settings);
                return;
            }

            try
            {
                showSettings(settings);
                var message = applySettings(settings);
                MessageBox.Show(string.IsNullOrEmpty(message) ? "Mapping finished!" : message, "OneDrive Mapper", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OneDrive Mapper - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void showSettings(Settings settings)
        {
            var dlg = new MainForm(settings);
            if (dlg.ShowDialog() == DialogResult.OK)
                settings.Save();
        }


        private static string applySettings(Settings settings)
        {
            using (var p = new Process())
            {
                p.StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"net use {settings.Drive.ToUpper()}: \"{settings.Url}\" /user:{settings.Username} {settings.Password} /persistent:yes",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                };
                p.Start();

                var result = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                return result;
            }
        }
    }
}
