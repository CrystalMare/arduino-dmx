using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace ArduinoDMX
{
    public partial class PortSelector : Form
    {

        private UserInterface ui;

        public PortSelector(UserInterface i)
        {
            ui = i;
            InitializeComponent();
            updateList();

        }

        private void updateList()
        {
            listBox1.Items.Clear();
            var ports = SerialPort.GetPortNames();
            foreach (string s in ports)
            {
                listBox1.Items.Add(s);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            ui.port = listBox1.SelectedItem.ToString();
            ui.newPort = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            updateList();
        }
    }
}
