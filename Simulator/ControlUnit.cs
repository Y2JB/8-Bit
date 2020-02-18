using System;
using System.Collections.Generic;
using System.IO;
using EightBitSystem;

namespace Simulator
{

    public class ControlUnit : IControlUnit
    {
        protected Dictionary<ControlLineId, ControlLine> controlLines = new Dictionary<ControlLineId, ControlLine>();

        public ControlUnit()
        {
            CreateControlLines();
        }


        public void LoadMicrocode(MemoryStream eeprom0, MemoryStream eeprom1, MemoryStream eeprom2)
        {
        }


        void CreateControlLines()
        {
            foreach (ControlLineId lineId in Enum.GetValues(typeof(ControlLineId)))
            {
                controlLines.Add(lineId, new ControlLine(lineId));
            }
        }

        public ControlLine GetControlLine(ControlLineId lineId)
        {
            return controlLines[lineId];
        }

        public void OnRisingEdge()
        {
        }

        public void OnFallingEdge()
        {
        }
    }

}
