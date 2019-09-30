using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskAllocationLibrary;

namespace GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void startReading()
        {
            mainFormOutput.Text = "Processing...";
        }

        private void clearOutput()
        {
            mainFormOutput.Text = "";
        }

        private void printRow(string text = "")
        {
            mainFormOutput.Text += $"{text}{Environment.NewLine}";
        }

        private void printWaitingTime()
        {
            mainFormOutput.Text += $"Please wait for 5 minutes...";
        }

        // Select the first config file in comboBox by default
        private void MainForm_Load(object sender, EventArgs e)
        {
            filePathsComboBox.SelectedIndex = 0;
        }

        // Get allocations from services on button-click
        private async void getAllocationsButton_Click(object sender, EventArgs e)
        {
            this.startReading();
            string fileUrlInputBox = urlInputBox.Text.Trim();
            string fileUrlComboxBox = filePathsComboBox.Text.Trim();
            string fileUrl = String.IsNullOrEmpty(fileUrlInputBox) ? fileUrlComboxBox : "";

            try {

                // Read a remote config file
                string fileContent = FileReader.ReadFromUrl(fileUrl);
                this.clearOutput();

                // Parse the config file
                ConfigFile configFile = new ConfigFile(fileContent);
                var allocInput = new TaskAllocationInput(configFile.Tasks, configFile.Processors, configFile.MaxDuration, configFile.RefFrequency, configFile.Coefficients);
                var allocations = new List<TaskAllocationOutput>();

                // Create all service clients
                var GAServiceClient = new AWSGAServiceReference.GAServiceClient();
                var heuristicClient = new AWSHeuristicServiceReference.HeuristicServiceClient();
                var sortMidClient = new AWSSortMidServiceReference.SortMidServiceClient();

                // Get allocations from SortMid Service
                var sortMidTask = Task.Factory.StartNew(() =>
                {
                    var sortMidAllocs = sortMidClient.GetAllocations(allocInput);
                    allocations.AddRange(sortMidAllocs);
                });

                // Get allocations from Heuristic Service
                var heuristicTask = Task.Factory.StartNew(() =>
                {
                    var heuristicAllocs = heuristicClient.GetAllocations(allocInput);
                    allocations.AddRange(heuristicAllocs);
                });

                // Get allocations from GA Service
                var GATask = Task.Factory.StartNew(() =>
                {
                    var GAAllocs = GAServiceClient.GetAllocations(allocInput);
                    allocations.AddRange(GAAllocs);
                });

                // UI feedback
                this.printWaitingTime();

                // Wait for response from all the services
                await Task.WhenAll(sortMidTask, GATask, heuristicTask);

                // Print correct allocations
                var topAllocations = allocations.OrderBy(a => a.EnergyConsumed).Take(3).ToList();
                string formattedAllocations = Visualizer.Allocations(topAllocations);
                this.printRow();
                this.printRow(formattedAllocations);

                // Close the service clients
                GAServiceClient.Close();
                heuristicClient.Close();
                GAServiceClient.Close();

            }
            catch (Exception ex) {
                this.clearOutput();
                this.printRow(ex.Message);
            }
        }
    }
}
