using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startReading();
            string fileUrl = "https://sit323sa.blob.core.windows.net/at2/TestA.txt";
            string fileContent = FileReader.ReadFromUrl(fileUrl);
            mainFormOutput.Text = fileContent;
        }

        private void startReading()
        {
            mainFormOutput.Text = "Processing...";
        }
    }
}
