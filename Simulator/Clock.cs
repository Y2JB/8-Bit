using System;
using System.Collections.Generic;
using EightBitSystem;

namespace Simulator
{

    public class Clock : IClock
    {
        public enum Mode
        {
            Halted,
            Running,
            Stepped
        }

        int frequencyHz;
        Mode mode;
        //ControlLine HltLine;

        public List<IClockConnectedComponent> clockConnectedComponents { get; private set; }

        public Clock()
        {
            //HltLine = controlUnit.GetControlLine(ControlLineId.HLT);
            frequencyHz = 1;
            mode = Mode.Halted;

            clockConnectedComponents = new List<IClockConnectedComponent>();
        }

        public void Step()
        {
            foreach(IClockConnectedComponent component in clockConnectedComponents)
            {
                component.OnRisingEdge();
            }

            foreach (IClockConnectedComponent component in clockConnectedComponents)
            {
                component.OnFallingEdge();
            }
        }

      
    }

}
