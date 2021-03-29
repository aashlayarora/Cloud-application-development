using System;
using System.Windows.Forms;

namespace Application_1
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            descTextBox.Text = "SIT323 T2 2020, Assessment Task 1";
        }

        // Close About Form
        private void OKbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
