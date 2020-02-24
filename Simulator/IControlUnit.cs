using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using EightBitSystem;

namespace Simulator
{

    public interface IControlUnit
    {
        IRegister InstructionRegister { get; set; }
        IRegister FlagsRegister { get; set; }
        ICounter MicrostepCounter { get; set; }
        ControlLine GetControlLine(ControlLineId lineId);
        void LoadMicrocode(string bank0RomFile, string bank1RomFile, string bank2RomFile);

        void OnControlStateUpdated();

        Point consoleXY { get; set; }
        void OutputState();
    }

}
