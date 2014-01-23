using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace ArduinoDMX
{
    /// <summary>
    /// Can connect to arduino and communicate using the DMXArduino protocol.
    /// This is also the main way to send requests to the Arduino.
    /// </summary>
    class SerialConnector
    {
        /// <summary>
        /// Represents the baudrate of this Connector
        /// </summary>
        readonly private int _baudrate;

        /// <summary>
        /// Represents the port of this Connector
        /// </summary>
        readonly private string _port;

        /// <summary>
        /// The physical connection to the Arduino
        /// </summary>
        private SerialPort _serial;

        /// <summary>
        /// Handles requests and timing with Arduino
        /// </summary>
        private Thread _connection;

        /// <summary>
        /// True if _connection must stay alive
        /// </summary>
        private Boolean _keepConnection = true;

        /// <summary>
        /// A list that contains requests, it follows FIFO rules.
        /// </summary>
        private LinkedList<DmxRequest> _requests = new LinkedList<DmxRequest>();

        /// <summary>
        /// Local DMX cache.
        /// </summary>
        private Dictionary<ushort, byte> _localDmx = new Dictionary<ushort, byte>();

        /// <summary>
        /// Represents a ready to send message to discover if COM port is valid.
        /// </summary>
        private readonly byte[] _discoverMessage = { (byte)Instruction.Discover };

        /// <summary>
        /// Represents a ready to send message to clear all DMX fixtures.
        /// </summary>
        private readonly byte[] _clearMessage = { (byte)Instruction.Clear, byte.MinValue, byte.MinValue, byte.MinValue, (byte)Instruction.Stop };

        /// <summary>
        /// Initializes a new instance of a SerialConnector. Can be used to communicate with an Arduino with DMX shield.
        /// </summary>
        /// <param name="port">The COM port</param>
        /// <param name="speed">The speed in baud</param>
        public SerialConnector(string port, int speed)
        {
            this._port = port;
            this._baudrate = speed;
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
        /// Initializes a new instance of SerialConnector. This can be used to test a connection.
        /// </summary>
        public SerialConnector() { }

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

        /// <summary>
        /// Disconnects arduino and clears up all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            _keepConnection = false;
            while (_connection.IsAlive);
            _serial.Close();
            _serial.Dispose();
        }

        /// <summary>
        /// Sends a request to the arduino to clear all DMX fixtures.
        /// </summary>
        public void ResetFixtures()
        {
            _requests.AddFirst(new DmxRequest(1, 1, Instruction.Clear));
        }

        /// <summary>
        /// Test if the COM port is valid. Can only work if _port == null
        /// </summary>
        /// <param name="port">The port to test</param>
        /// <param name="speed">The speed of this port</param>
        /// <returns>True if device is valid</returns>
        public Boolean TestConnection(string port, int speed)
        {
            //Check if initialized via normal constructor
            if (_port != null) throw new ApplicationException("Can't run method in normal Constructed mode");

            try
            {
                //Initialize serial port
                _serial = new SerialPort(port, speed, Parity.None, 8, StopBits.One);
                _serial.Open();
                _serial.Write(_discoverMessage, 0, _discoverMessage.Length);
                for (int i = 0; i < 1000; i += 50)
                {
                    if (_serial.BytesToRead >= 1)
                    {
                        Thread.Sleep(100);
                        int data = _serial.ReadByte();
                        Console.WriteLine(data);
                        if (data == 44)
                        {
                            _serial.Close();
                            _serial.Dispose();
                            return true;
                        }
                    }
                    Thread.Sleep(50);
                }
                _serial.Close();
                _serial.Dispose();
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
