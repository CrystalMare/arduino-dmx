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
        /// 
        /// </summary>
        private PortSelector _selector;

        public string port;
        public bool newPort;

        private bool active;

        private const ushort LedPunchChannel = 1;

        public UserInterface()
        {
            InitializeComponent();
            setLabel(false);

            colorWheel1.Saturation = 255;
            colorWheel1.Lightness = 127;
        }

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

        private int Map(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void exitApplication()
        {
            if (_arduino != null) _arduino.Dispose();
            
            Application.Exit();
        }

        private void UserInterface_FormClosed(object sender, FormClosedEventArgs e)
        {
            exitApplication();
        }

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

        private void colorWheel1_MouseUp(object sender, EventArgs e)
        {
            if (!active) return;
            var c = ColorMath.HslToRgb(new HslColor(colorWheel1.Hue, colorWheel1.Saturation, colorWheel1.Lightness));
            _arduino.DmxSet(LedPunchChannel + 0, c.R);
            _arduino.DmxSet(LedPunchChannel + 1, c.G);
            _arduino.DmxSet(LedPunchChannel + 2, c.B);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (!active) return;
            if (trackBar1.Value == 0) _arduino.DmxSet(LedPunchChannel + 4, 0);
            else _arduino.DmxSet(LedPunchChannel + 4, (byte)Map(trackBar1.Value, 0, 100, 16, 255));
        }

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

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_arduino != null) _arduino.Dispose();
            active = false;
            setLabel(active);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!active) return;
            if (!checkBox2.Checked) _arduino.DmxSet(LedPunchChannel + 5, 0);
            else _arduino.DmxSet(LedPunchChannel + 5, 255); 
        }
    }
}
