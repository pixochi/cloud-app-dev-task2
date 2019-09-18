﻿using System;
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

        private void getAllocationsButton_Click(object sender, EventArgs e)
        {
            this.startReading();
            string fileUrl = urlInputBox.Text.Trim();

            try {
                string fileContent = FileReader.ReadFromUrl(fileUrl);
                this.clearOutput();

                ConfigFile configFile = new ConfigFile(fileContent);
                this.printRow($"Max duration: {configFile.MaxDuration}");
                this.printRow($"Ref freq.: {configFile.RefFrequency}");

                this.printRow();
                this.printRow("Tasks:");

                foreach (var n in configFile.Tasks) {
                    printRow($"{n.Key}, {n.Value.ToString()}");
                }

                this.printRow();
                this.printRow("Processors:");

                foreach (var n in configFile.Processors) {
                    printRow($"{n.Key}, {n.Value.ToString()}");
                }

                this.printRow();
                this.printRow("From Service:");
                var client = new AllocServiceReference.AllocServiceClient();
                var allocInput = new AllocServiceReference.AllocInput(configFile.Tasks, configFile.Processors, configFile.MaxDuration, configFile.RefFrequency, configFile.Coefficients);
                List<AllocServiceReference.AllocOutput> allocations = client.GetAllocationsHeuristic(allocInput);

                string formattedAllocations = Visualizer.Allocations(allocations);
                this.printRow(formattedAllocations);

            }
            catch (Exception ex) {
                this.clearOutput();
                this.printRow(ex.Message);
            } 
        }
    }
}
