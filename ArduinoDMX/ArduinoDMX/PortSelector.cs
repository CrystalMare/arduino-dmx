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

        private UserInterface _main;

        public PortSelector(UserInterface i)
        {

            InitializeComponent();
            UpdateList();
            _main = i;
        }


        private void UpdateList()
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
            if (IsValidDevice(listBox1.SelectedItem.ToString(), 9600))
            {
                _main.port = listBox1.SelectedItem.ToString();
                _main.newPort = true;
                this.Close();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        private Boolean IsValidDevice(string port, int speed)
        {
            
            int i = 0;
            while (i < 3)
            {
                if (new SerialConnector().TestConnection(listBox1.SelectedItem.ToString(), speed)) return true;
                else i++;
            }
            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
