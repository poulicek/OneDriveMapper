using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneDriveMapper
{
    public partial class MainForm : Form
    {
        private readonly Settings settings;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(Settings settings) : this()
        {
            this.settings = settings;
            this.bind();
        }


        private void bind()
        {
            this.txtUrl.DataBindings.Add("Text", this.settings, "Url");
            this.txtUsername.DataBindings.Add("Text", this.settings, "Username");
            this.txtPassword.DataBindings.Add("Text", this.settings, "Password");
            this.txtDrive.DataBindings.Add("Text", this.settings, "Drive");
        }
    }
}
