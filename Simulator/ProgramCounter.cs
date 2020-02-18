using System;
using EightBitSystem;

namespace Simulator
{

    public class ProgramCounter : ICounter
    {
        public bool CountEnabled { get { return countEnableLine.State; } }

        public byte MaxValue { get { return 255; } }

        public byte Value { get; private set; }

        public IBus Bus { get; private set; }

        ControlLine busOutputLine;
        ControlLine countEnableLine;
        ControlLine loadLine;

        public ProgramCounter(IBus bus, IControlUnit controlUnit)
        {
            this.Bus = bus;
            busOutputLine = controlUnit.GetControlLine(ControlLineId.PC_OUT);
            countEnableLine = controlUnit.GetControlLine(ControlLineId.PC_ENABLE);
            loadLine = controlUnit.GetControlLine(ControlLineId.PC_IN);          
        }

        public void Reset()
        {
            Value = 0;
        }

        public void OnRisingEdge()
        {
            if(loadLine.State == true)
            {
                Value = Bus.Value;
                return;
            }

            if (busOutputLine.State == true)
            {
                Bus.Driver = this;
            }

            if(CountEnabled)
            {
                Value++;

                if(Value > MaxValue)
                {
                    Value = 0;
                }
            }
        }

        public void OnFallingEdge()
        {
        }

    }

}
