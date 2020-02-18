using System;
using System.Collections.Generic;
using System.IO;

namespace Simulator
{

    public class ControlUnit : IControlUnit
    {
        protected List<IControlLine> controlLines = new List<IControlLine>();

        public void LoadMicrocode(MemoryStream eeprom0, MemoryStream eeprom1, MemoryStream eeprom2)
        {
        }

        public void Reset()
        {
        }

        public void OnClockPulse()
        {
        }

    }

}
