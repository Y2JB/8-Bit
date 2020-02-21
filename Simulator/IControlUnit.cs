using System;
using System.Collections.Generic;
using System.IO;
using EightBitSystem;

namespace Simulator
{

    public interface IControlUnit : IClockConnectedComponent
    {
        IRegister InstructionRegister { get; set; }
        ICounter MicrostepCounter { get; set; }
        ControlLine GetControlLine(ControlLineId lineId);
        public void LoadMicrocode();
    }

}
