using System;
using System.Collections.Generic;
using System.IO;
using EightBitSystem;

namespace Simulator
{

    public interface IControlUnit : IClockConnectedComponent
    {
        ControlLine GetControlLine(ControlLineId lineId);
        public void LoadMicrocode(MemoryStream eeprom0, MemoryStream eeprom1, MemoryStream eeprom2);
    }

}
