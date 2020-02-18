using System;
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
        ControlLine HltLine;


        public Clock(IControlUnit controlUnit)
        {
            HltLine = controlUnit.GetControlLine(ControlLineId.HLT);
            frequencyHz = 1;
            mode = Mode.Halted;
        }

        public void Step()
        {

        }

        public void OnHigh()
        {

        }

        public void OnLow()
        {

        }
    }

}
