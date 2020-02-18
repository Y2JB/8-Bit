using System;

namespace Simulator
{

    public class ProgramCounter : ICounter
    {
        public bool CountEnabled { get; set; }

        public byte MaxValue { get; }

        public byte Value { get; private set; }

        IControlLine busOutputLine;
        IControlLine countEnableLine;
        IControlLine loadLine;
        IControlLine resetLine;

        public ProgramCounter()
        {
        }

        public void Reset()
        {
            Value = 0;
        }

        public void OnClockPulse()
        {
            if(CountEnabled)
            {
                Value++;

                if(Value > MaxValue)
                {
                    Value = 0;
                }
            }
        }

        public void Load()
        {
        }

    }

}
