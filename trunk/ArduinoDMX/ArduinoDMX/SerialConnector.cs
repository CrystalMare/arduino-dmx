using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

namespace ArduinoDMX
{
    class SerialConnector
    {
        private SerialPort _serial;
        private Thread _connection;

        private LinkedList<DmxRequest> _requests = new LinkedList<DmxRequest>();


        /// <summary>
        /// Initializes a new instance of a SerialConnector. Can be used to communicate with an Arduino with DMX shield.
        /// </summary>
        /// <param name="port">The COM port</param>
        /// <param name="speed">The speed in baud</param>
        public SerialConnector(string port, int speed)
        {
            //TODO: Add a way to select port on startup

            //Initialize serial port
            _serial = new SerialPort(port, speed, Parity.None, 8, StopBits.One);
            //Add event handler to print an error from arduino
            _serial.DataReceived += new SerialDataReceivedEventHandler(_serial_DataReceived);
            _serial.Open();

            //Start thread to handle requests and timing.
            _connection = new Thread(new ThreadStart(SendData));
            _connection.Start();
        }

        private void DmxSend(ushort channel, byte fixture, Instruction instruction) {
            byte[] chan = BitConverter.GetBytes(channel);
            byte[] message = { (byte)Instruction.Set, chan[0], chan[1], fixture, (byte)Instruction.Stop };
            _serial.Write(message, 0, message.Length);
        }

        public void DmxSet(ushort channel, byte fixture)
        {
            if (channel > 512 || fixture > 255)
            {
                throw new ApplicationException("Channel or fixture out of bounds");
            }

            var req = new DmxRequest(channel, fixture, Instruction.Set);
            _requests.AddLast(req);
        }

        private void SendData()
        {
            while (true)
            {
                if (_requests.Count <= 0) continue;
                var req = _requests.First.Value; _requests.RemoveFirst();
                DmxSend(req.Channel, req.Fixture, req.Instruction);
                Thread.Sleep(20);
            }
        }

        private void _serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine(_serial.ReadLine());
        }
    }
}
