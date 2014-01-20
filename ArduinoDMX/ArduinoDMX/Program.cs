using System;
using System.Windows.Forms;

namespace ArduinoDMX
{
    class Program
    {
    
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.Run(new UserInterface());
        }
    }
}
