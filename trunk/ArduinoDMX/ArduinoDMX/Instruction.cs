using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoDMX
{
    public enum Instruction : byte
    {
        Set = 53,
        Clear = 43,
        Stop = 23,
        Discover = 44
    }
}
