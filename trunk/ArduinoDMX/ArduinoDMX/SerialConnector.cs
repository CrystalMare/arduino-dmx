using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;

namespace ArduinoDMX
{
    class SerialConnector
    {
        private SerialPort _serial;

        public SerialConnector(string port, int speed)
        {
            _serial = new SerialPort(port, speed, Parity.None, 8, StopBits.One);
            _serial.DataReceived += new SerialDataReceivedEventHandler(_serial_DataReceived);
            _serial.Open();
        }

        public void dmxSend(ushort channel, byte fixture, Instruction instruction) {
            byte[] chan = BitConverter.GetBytes(channel);
            byte[] message = { 53, chan[0], chan[1], fixture, 23 };
            _serial.Write(message, 0, message.Length);
        }

        private void _serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            Console.WriteLine("Receiving data.");
            Console.WriteLine(_serial.ReadLine());
        }
    }
}
