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


        public MicrostepCounter(IClock clock)
        {
            clock.clockConnectedComponents.Add(this);
        }

        public void Reset()
        {
            Value = 0;
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
        }


        public void OutputState()
        {

        }
    }

}
