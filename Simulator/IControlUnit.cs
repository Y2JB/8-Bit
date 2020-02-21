using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using EightBitSystem;

namespace Simulator
{

    public interface IControlUnit : IClockConnectedComponent
    {
        IRegister InstructionRegister { get; set; }
        IRegister FlagsRegister { get; set; }
        ICounter MicrostepCounter { get; set; }
        ControlLine GetControlLine(ControlLineId lineId);
        void LoadMicrocode();


        Point consoleXY { get; set; }
        void OutputState();
    }

}
