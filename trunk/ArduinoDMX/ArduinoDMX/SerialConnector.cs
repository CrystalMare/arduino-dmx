using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace ArduinoDMX
{
    class SerialConnector
    {
        private SerialPort _serial;
        private Thread _connection;
        private Boolean _keepConnection = true;

        private LinkedList<DmxRequest> _requests = new LinkedList<DmxRequest>();

        private Dictionary<ushort, byte> _localDmx = new Dictionary<ushort, byte>();

        private readonly byte[] _discoverMessage = { (byte)Instruction.Discover, (byte)Instruction.Stop };
        private readonly byte[] _clearMessage = { (byte)Instruction.Clear, byte.MinValue, byte.MinValue, byte.MinValue, (byte)Instruction.Stop };

        /// <summary>
        /// Initializes a new instance of a SerialConnector. Can be used to communicate with an Arduino with DMX shield.
        /// </summary>
        /// <param name="port">The COM port</param>
        /// <param name="speed">The speed in baud</param>
        public SerialConnector(string port, int speed)
        {
            //Initialize serial port
            _serial = new SerialPort(port, speed, Parity.None, 8, StopBits.One); 
            //Add event handler to print an error from arduino
            _serial.DataReceived += new SerialDataReceivedEventHandler(_serial_DataReceived);
            _serial.Open();

            //Start thread to handle requests and timing.
            _connection = new Thread(new ThreadStart(HandleRequests));
            _connection.Start();
        }

        /// <summary>
        /// Updates the local dmx memory
        /// </summary>
        /// <param name="channel">The channel that is affected</param>
        /// <param name="fixture">The fixture of the affected channel</param>
        private void UpdateBuffer(ushort channel, byte fixture)
        {
            if (!_localDmx.ContainsKey(channel)) _localDmx.Add(channel, fixture);
            else
            {
                _localDmx.Remove(channel);
                _localDmx.Add(channel, fixture);
            }
        }

        /// <summary>
        /// Sends a dmx request through the serial port, it will must be ran on a seperate thread.
        /// </summary>
        /// <param name="channel">The channel that is affected</param>
        /// <param name="fixture">The fixture of the affected channel</param>
        /// <param name="instruction">The instruction for the request, valid instructions are: Set, Clear.</param>
        private void DmxSend(ushort channel, byte fixture, Instruction instruction) {
            if (channel > 512) throw new ArgumentOutOfRangeException("channel", channel, "Channel has to be in DMX Spectrum, 1-512");
            switch (instruction)
            {
                case Instruction.Set:
                    byte[] chan = BitConverter.GetBytes(channel);
                    byte[] message = { (byte)instruction, chan[0], chan[1], fixture, (byte)Instruction.Stop };
                    _serial.Write(message, 0, message.Length);
                    UpdateBuffer(channel, fixture);
                    break;
                case Instruction.Stop:
                    throw new ArgumentException("Invalid instruction", "instruction");
                case Instruction.Clear:
                    _serial.Write(_clearMessage, 0, _clearMessage.Length);
                    _localDmx.Clear();
                    break;
            }
        }

        /// <summary>
        /// Adds a request to the Dmx Request Queue
        /// </summary>
        /// <param name="channel">The channel that is affected</param>
        /// <param name="fixture">The fixtured of the affected channel</param>
        public void DmxSet(ushort channel, byte fixture)
        {
            if (channel > 512 || fixture > 255)
            {
                throw new ApplicationException("Channel or fixture out of bounds");
            }

            var req = new DmxRequest(channel, fixture, Instruction.Set);
            _requests.AddLast(req);
        }

        /// <summary>
        /// This method loops and handles requests when they appear
        /// </summary>
        private void HandleRequests()
        {
            while (_keepConnection)
            {
                if (_requests.Count <= 0) continue;

                var req = _requests.First.Value;

                if (req.Instruction == Instruction.Clear) _requests.Clear();
                else _requests.RemoveFirst();

                DmxSend(req.Channel, req.Fixture, req.Instruction);
                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// Called when a serial message is received 
        /// </summary>
        /// <param name="sender">The origin of the message</param>
        /// <param name="e"></param>
        private void _serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine(_serial.ReadLine());
        }

        public void Dispose()
        {
            _keepConnection = false;
            while (_connection.IsAlive);
            _serial.Close();
            _serial.Dispose();
        }

        public void ResetFixtures()
        {
            _requests.AddFirst(new DmxRequest(1, 1, Instruction.Clear));
        }

        public Boolean TestConnection()
        {
            _serial.Write(_discoverMessage, 0, _discoverMessage.Length);
            return true;
        }
    }
}
