using System;
using System.Drawing;
using EightBitSystem;

namespace Simulator
{

    public class MicrostepCounter : ICounter
    {
        public bool CountEnabled { get { return true; } }

        public byte MaxValue { get { return 7; } }

        public byte Value { get; private set; }

        public Point consoleXY { get; set; }

        IControlUnit controlUnit;

        public MicrostepCounter(IClock clock, IControlUnit controlUnit)
        {
            clock.clockConnectedComponents.Add(this);
            this.controlUnit = controlUnit;
        }

        public void Reset()
        {
            Value = 0;

            // The microstep being updated should be immedietley refelected by the control unit
            controlUnit.OnControlStateUpdated();
        }


        public void OnRisingEdge()
        {
        }


        // Microsteps drive the control unit process. The control unit sets things up when the clock is low, ready for the system to operate the next clock pulse high
        public void OnFallingEdge()
        {
            Value++;

            if (Value > MaxValue)
            {
                Value = 0;
            }

            // The microstep being updated should be immedietley refelected by the control unit
            controlUnit.OnControlStateUpdated();
        }


        public void OutputState()
        {

        }
    }

}
