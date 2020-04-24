using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace OneDriveMapper
{
    public partial class SettingsForm : Form
    {
        private readonly Settings settings;

        public SettingsForm()
        {
            InitializeComponent();
        }

        public SettingsForm(Settings settings) : this()
        {
            this.settings = settings;
            this.bind();
        }


        private void bind()
        {
            this.txtCID.DataBindings.Add("Text", this.settings, "CID");
            this.txtUsername.DataBindings.Add("Text", this.settings, "Username");
            this.txtPassword.DataBindings.Add("Text", this.settings, "Password");
            this.txtDrive.DataBindings.Add("Text", this.settings, "Drive");
            this.chbAutorun.DataBindings.Add("Checked", this.settings, "Autorun");
        }

        private void cidPreview_Click(object sender, EventArgs e)
        {
            Process.Start("https://onedrive.live.com/");
        }
    }
}
