using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ArduinoDMX
{
    class ArduinoDmx
    {
        SerialConnector arduino = new SerialConnector("COM3", 9600);

        public ArduinoDmx()
        {
            //Initialize
            Start();
        }

        private void Start()
        {
            Application.Run();
        }
    }
}
