using System;
using EightBitSystem;

namespace Simulator
{

    public class Alu : IAlu
    {
        // The ALU is hardwired into the A and B registers. It is constantly adding or subtracting their values
        IRegister aReg;
        IRegister bReg;

        public IBus Bus { get; private set; }

        public bool Carry { get { return false; } }
        public bool Zero { get { return false; } }

        ControlLine busOutputLine;
        ControlLine subLine;

        public byte Value
        {
            get
            {
                if(subLine.State == true)
                {
                    return (byte) (aReg.Value - bReg.Value);
                }
                else
                {
                    return (byte) (aReg.Value + bReg.Value);
                }
            }
        }

        public Alu(IControlUnit controlUnit, IBus bus, IRegister aReg, IRegister bReg)
        {
            Bus = bus;

            this.aReg = aReg;
            this.bReg = bReg;

            busOutputLine = controlUnit.GetControlLine(ControlLineId.SUM_OUT);
            subLine = controlUnit.GetControlLine(ControlLineId.SUBTRACT);
        }


        public void Reset()
        {
        }

        public void OnClockPulse()
        {
        }
    }

}
