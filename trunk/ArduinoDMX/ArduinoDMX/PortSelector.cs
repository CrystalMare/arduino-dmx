using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace ArduinoDMX
{
    public partial class PortSelector : Form
    {
        /// <summary>
        /// The main userinterface
        /// </summary>
        private UserInterface _main;

        /// <summary>
        /// Represents a Form that the user can use to select a COM Port
        /// </summary>
        /// <param name="i">The UserInterface from wich this is called.</param>
        public PortSelector(UserInterface i)
        {
            InitializeComponent();
            UpdateList();
            _main = i;
        }

        /// <summary>
        /// Checks for COM changes and updates the list.
        /// </summary>
        private void UpdateList()
        {
            listBox1.Items.Clear();
            var ports = SerialPort.GetPortNames();
            foreach (string s in ports)
            {
                listBox1.Items.Add(s);
            }
        }

        /// <summary>
        /// Called when 'Connect' is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Called when 'Refresh' is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        /// <summary>
        /// Returns a value indictation if the specified COM port is valid.
        /// </summary>
        /// <param name="port">The COM port to test</param>
        /// <param name="speed">The speed of the COM port</param>
        /// <returns>True if device is valid</returns>
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

        /// <summary>
        /// Called when 'Cancel' is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
