﻿namespace ArduinoDMX
{
    class DmxRequest
    {
        /// <summary>
        /// A container that contains all information needed for a DMX Request
        /// </summary>
        /// <param name="channel">The channel that is affected</param>
        /// <param name="fixture">The value of the channel</param>
        /// <param name="i">The instruction</param>
        public DmxRequest(ushort channel, byte fixture, Instruction i)
        {
            Channel = channel;
            Fixture = fixture;
            Instruction = i;
        }

        /// <summary>
        /// An emtpy DMX Request that can contain all information for a DMX Request
        /// </summary>
        public DmxRequest() { }

        /// <summary>
        /// The Channel of this request
        /// </summary>
        public ushort Channel           { get; set; }

        /// <summary>
        /// The Fixture of this channel
        /// </summary>
        public byte Fixture             { get; set; }

        /// <summary>
        /// The Instruction type of this request
        /// </summary>
        public Instruction Instruction  { get; set; }
    }
}
