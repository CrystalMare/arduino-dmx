using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Unclassified.Drawing;

namespace ArduinoDMX
{
    public partial class UserInterface : Form
    {

        private SerialConnector _arduino;

        private readonly ushort LedPunchChannel = 1;

        public UserInterface()
        {
            InitializeComponent();

            colorWheel1.Lightness = 127;
            colorWheel1.Saturation = 255;

            _arduino = new SerialConnector("COM3", 9600);

            _arduino.DmxSet(7, 255);
        }

        private void colorWheel1_MouseUp(object sender, MouseEventArgs e)
        {
            var c = ColorMath.HslToRgb(new HslColor(colorWheel1.Hue, colorWheel1.Saturation, colorWheel1.Lightness));

            _arduino.DmxSet(LedPunchChannel, c.R);
            _arduino.DmxSet((ushort)(LedPunchChannel + 1), c.G);
            _arduino.DmxSet((ushort)(LedPunchChannel + 2), c.B);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            byte value = (byte)Map((int)numericUpDown1.Value, 0, 100, 16, 255);
            _arduino.DmxSet((ushort)(LedPunchChannel + 4), value);
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
            _arduino.Dispose();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _arduino.ResetFixtures();
            _arduino.DmxSet(7, 255);
        }

        private void UserInterface_FormClosed(object sender, FormClosedEventArgs e)
        {
            exitApplication();
        }
    }
}
