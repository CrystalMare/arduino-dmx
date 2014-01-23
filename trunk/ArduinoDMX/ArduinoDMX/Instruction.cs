namespace ArduinoDMX
{
    /// <summary>
    /// Represents an Instruction byte for a Dmx Request
    /// </summary>
    public enum Instruction : byte
    {
        Set = 53,
        Clear = 43,
        Stop = 23,
        Discover = 44
    }
}
