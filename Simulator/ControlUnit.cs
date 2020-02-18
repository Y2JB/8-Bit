using System;
using System.Collections.Generic;
using System.IO;

namespace Simulator
{

    public class ControlUnit : IControlUnit
    {
        protected Dictionary<int, IControlLine> controlLines = new Dictionary<int, IControlLine>();
        protected Dictionary<int, IControlLine> resetLines = new Dictionary<int, IControlLine>();

        public void LoadMicrocode(MemoryStream eeprom0, MemoryStream eeprom1, MemoryStream eeprom2)
        {
        }


    }

}
