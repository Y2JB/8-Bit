using System;
using EightBitSystem;

namespace Simulator
{
    public class Register : IRegister
    {
        SystemRegister id;

        public IBus Bus { get; private set; }

        public byte Value { get; private set; }
        
        public string BinarytValue { get { return Convert.ToString(Value, 2).PadLeft(8, '0'); } }

        ControlLine busOutputLine;
        ControlLine busInputLine;

        public Register(SystemRegister id, IBus bus, IControlUnit controlUnit)
        {
            this.id = id;
            Bus = bus;
            Value = 0;

            switch(id)
            {
                case SystemRegister.A:
                    busOutputLine = controlUnit.GetControlLine(ControlLineId.A_REG_OUT);
                    busInputLine = controlUnit.GetControlLine(ControlLineId.A_REG_IN);
                    break;

                case SystemRegister.B:
                    busOutputLine = controlUnit.GetControlLine(ControlLineId.B_REG_OUT);
                    busInputLine = controlUnit.GetControlLine(ControlLineId.B_REG_IN);
                    break;

                case SystemRegister.MAR:
                    busOutputLine = null;
                    busInputLine = controlUnit.GetControlLine(ControlLineId.MAR_IN);
                    break;

                case SystemRegister.IR:
                    busOutputLine = null;
                    busInputLine = controlUnit.GetControlLine(ControlLineId.IR_IN);
                    break;

                case SystemRegister.IR_PARAM:
                    busOutputLine = controlUnit.GetControlLine(ControlLineId.IR_PARAM_OUT);
                    busInputLine = controlUnit.GetControlLine(ControlLineId.IR_PARAM_IN);
                    break;

                case SystemRegister.OUT:
                    busOutputLine = null;
                    busInputLine = controlUnit.GetControlLine(ControlLineId.OUT_REG_IN);
                    break;

                default:
                    throw new ArgumentException("missing reg type");
            }
        }


        public void SetBit(int bit, bool value)
        {
            if (bit < 0 || bit > 7)
            {
                throw new ArgumentException("Bit must be 0 - 7");
            }

            byte mask = (byte) (1 << bit);
            Value |= mask;
        }


        public bool GetBit(int bit)
        {
            if(bit < 0 ||  bit > 7)
            {
                throw new ArgumentException("Bit must be 0 - 7");
            }

            int mask = (byte) (1 << bit);
            return (Value & mask) != 0;
        }


        public void Reset()
        {
            Value = 0;
        }


        public void OnRisingEdge()
        {
            if(busInputLine.State == true)
            {
                Value = Bus.Value;
                return;
            }

            if(busOutputLine.State == true)
            {
                Bus.Driver = this;
            }
        }


        public void OnFallingEdge() { }
       
    }
}
