using System;
using System.Drawing;
using System.Runtime.InteropServices;
using EightBitSystem;
using static Simulator.IDisplayComponent;

namespace Simulator
{
    public class Register : IRegister, IBusConnectedComponent
    {
        SystemRegister id;

        public IBus Bus { get; private set; }
        public string Name { get { return id.ToString(); } }

        public byte Value { get; private set; }        
        public string BinaryValue { get { return Convert.ToString(Value, 2).PadLeft(8, '0'); } }

        public Point ConsoleXY { get; set; }

        ControlLine busOutputLine;
        ControlLine busInputLine;

        IControlUnit controlUnit;

        public Register(SystemRegister id, IClock clock, IBus bus, IControlUnit controlUnit)
        {
            this.id = id;
            Bus = bus;
            Value = 0;

            this.controlUnit = controlUnit;

            clock.AddConnectedComponent(this);

            switch (id)
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

            // Setup the callback for when the bus output line goes high or low. Depending on which, we either start or stop driving the bus
            if(busOutputLine != null)
            {
                busOutputLine.onTransition = () =>
                {
                    if (busOutputLine.State == true)
                    {
                        Bus.Driver = this;
                    }
                    else
                    {
                        if(Bus.Driver == this)
                        {
                            Bus.Driver = null;
                        }
                    }
                    return true;
                };
            }
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

            if (Bus.Driver == this)
            {
                Bus.Driver = null;
            }

            // The IR being updated should be immedietley refelected by the control unit
            if (id == SystemRegister.IR)
            {
                controlUnit.OnControlStateUpdated();
            }
        }


        public void OnRisingEdge()
        {
            if(busInputLine != null && busInputLine.State == true)
            {
                Value = Bus.Value;

                // The IR being updated should be immedietley refelected by the control unit
                if(id == SystemRegister.IR)
                {
                    controlUnit.OnControlStateUpdated();
                }
                return;
            }

            if(busOutputLine != null && busOutputLine.State == true)
            {
                Bus.Driver = this;
            }
        }


        public void OnFallingEdge()
        {
        }


        public void OutputState(ValueFormat format)
        {
            Console.ForegroundColor = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ConsoleColor.Black : ConsoleColor.White;
            if (Bus.Driver == this)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if(busInputLine != null && busInputLine.State)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y);
            Console.Write("|-----------------------|");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write("|                       |");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);

            switch (format)
            {
                case ValueFormat.Hex:
                    Console.Write(String.Format("|{0}: 0x{1:X2}", id.ToString(), Value));
                    break;

                case ValueFormat.Decimal:
                    Console.Write(String.Format("|{0}: {1}", id.ToString(), Value));
                    break;

                case ValueFormat.Binary:
                    Console.Write(String.Format("|{0}: {1}", id.ToString(), BinaryValue));
                    break;
            } 

            // Yes this should be done with inheritence...
            if (id == SystemRegister.IR)
            {
                OpCode opCode = (OpCode) (Value >> 3);
                GeneralPurposeRegisterId reg = (GeneralPurposeRegisterId)(Value & 0x07);            
                Console.Write(String.Format(" {0}",opCode.ToString()));

                if(Enum.IsDefined(reg.GetType(), reg))
                {
                    Console.Write(String.Format(" {0}", reg.ToString()));
                }
            }
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }

    }
}
