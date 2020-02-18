using System;
using System.Collections.Generic;
using System.IO;

namespace Simulator
{

    public interface IControlUnit
    {
        public void LoadMicrocode(MemoryStream eeprom0, MemoryStream eeprom1, MemoryStream eeprom2);
    }

}
