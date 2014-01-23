using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Unclassified.Drawing;

namespace ArduinoDMX
{
    public partial class UserInterface : Form
    {
        /// <summary>
        /// The arduino to use for sending DMX Fixtures.
        /// </summary>
        private SerialConnector _arduino;

        /// <summary>
        /// The interface the user can use to select a COM port
        /// </summary>
        private PortSelector _selector;

        /// <summary>
        /// The COM port that will be connected to next time StartArduino() is called.
        /// </summary>
        public string port;

        /// <summary>
        /// True if a new connection must be started.
        /// </summary>
        public bool newPort;

        /// <summary>
        /// True if there is a connection alive.
        /// </summary>
        private bool active;

        /// <summary>
        /// Represents the Address on wich the LedPunch Pro is set.
        /// </summary>
        private const ushort LedPunchChannel = 1;

        /// <summary>
        /// Creates a new UserInterface, this is also the primary interface for the application.
        /// </summary>
        public UserInterface()
        {
            InitializeComponent();
            setLabel(false);

            colorWheel1.Saturation = 255;
            colorWheel1.Lightness = 127;
        }

        /// <summary>
        /// Sets status label to value indication state of application.
        /// </summary>
        /// <param name="state">The state of the connection</param>
        private void setLabel(bool state)
        {
            if (state)
            {
                arduinoStatusLabel.ForeColor = Color.Green;
                arduinoStatusLabel.Text = "Ready";
            }
            else
            {
                arduinoStatusLabel.ForeColor = Color.Red;
                arduinoStatusLabel.Text = "Not Ready";
            }
        }

        /// <summary>
        /// Returns a value that can be re-mapped
        /// </summary>
        /// <param name="x">Value to be mapped</param>
        /// <param name="in_min">Input lower bounds</param>
        /// <param name="in_max">Input upper bounds</param>
        /// <param name="out_min">Output lower bounds</param>
        /// <param name="out_max">Output upper bounds</param>
        /// <returns>The re-mapped value</returns>
        private int Map(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        /// <summary>
        /// Called when menu button "Application > Exit" is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Releases all resources and exists application.
        /// </summary>
        private void exitApplication()
        {
            if (_arduino != null) _arduino.Dispose();
            
            Application.Exit();
        }

        /// <summary>
        /// Called when user pressed X on the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserInterface_FormClosed(object sender, FormClosedEventArgs e)
        {
            exitApplication();
        }

        /// <summary>
        /// Called after Interface is ready, and shown to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserInterface_Shown(object sender, EventArgs e)
        {
            _selector = new PortSelector(this);
            _selector.ShowDialog();
            if (newPort)
            {
                newPort = false;
                StartArduino(port, 9600);
            }
        }

        /// <summary>
        /// Starts a new connection on specified COM port and speed.
        /// </summary>
        /// <param name="port">The COM port to connect to</param>
        /// <param name="speed">The speed of this port</param>
        private void StartArduino(string port, int speed)
        {
            _arduino = new SerialConnector(port, speed);
            Thread.Sleep(100);
            _arduino.ResetFixtures();
            Thread.Sleep(100);
            _arduino.DmxSet((ushort)(LedPunchChannel) + 6, 255);
            colorWheel1.Saturation = 255;
            colorWheel1.Lightness = 127;
            active = true;
            setLabel(active);
        }

        /// <summary>
        /// Called when user released mouse after clicking colorwheel
        /// Sends requests to Arduino
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorWheel1_MouseUp(object sender, EventArgs e)
        {
            if (!active) return;
            var c = ColorMath.HslToRgb(new HslColor(colorWheel1.Hue, colorWheel1.Saturation, colorWheel1.Lightness));
            _arduino.DmxSet(LedPunchChannel + 0, c.R);
            _arduino.DmxSet(LedPunchChannel + 1, c.G);
            _arduino.DmxSet(LedPunchChannel + 2, c.B);
        }

        /// <summary>
        /// Called if value on the trackbar is changed.
        /// Sends requests to the arduino.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (!active) return;
            if (trackBar1.Value == 0) _arduino.DmxSet(LedPunchChannel + 4, 0);
            else _arduino.DmxSet(LedPunchChannel + 4, (byte)Map(trackBar1.Value, 0, 100, 16, 255));
        }

        /// <summary>
        /// Called when menu item "Arudino > Connect" is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.active = false;
            _selector = new PortSelector(this);
            _selector.ShowDialog();
            if (newPort)
            {
                newPort = false;
                StartArduino(port, 9600);
            }
        }

        /// <summary>
        /// Called when menu item "Arduino > Disconnect" is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_arduino != null) _arduino.Dispose();
            active = false;
            setLabel(active);
        }

        /// <summary>
        /// Called if the checkbox has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!active) return;
            if (!checkBox2.Checked) _arduino.DmxSet(LedPunchChannel + 5, 0);
            else _arduino.DmxSet(LedPunchChannel + 5, 255); 
        }
    }
}
