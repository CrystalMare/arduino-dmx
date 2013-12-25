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

    }
}
