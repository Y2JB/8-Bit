using System;
using System.Drawing;
using System.Runtime.InteropServices;
using EightBitSystem;
using static Simulator.IDisplayComponent;

namespace Simulator
{

    public class Alu : IAlu
    {
        // The ALU is hardwired into the A and B registers. It is constantly adding or subtracting their values
        IRegister aReg;
        IRegister bReg;

        public IBus Bus { get; private set; }
        public string Name { get { return "ALU"; } }

        public bool Carry { get; private set; }
        public bool Zero { get; private set; }

        ControlLine busOutputLine;
        ControlLine subLine;

        public Point ConsoleXY { get; set; }

        public byte Value
        {
            get
            {
                int val;
                if (subLine.State == true)
                {
                    val = aReg.Value - bReg.Value;
                }
                else
                {
                    val = aReg.Value + bReg.Value;
                }
                Zero = (val == 0);              
                Carry = (val > 255 || val < 0);

                if (Carry) val >>= 8;

                return (byte) val;  
            }
        }
        public string BinaryValue { get { return Convert.ToString(Value, 2).PadLeft(8, '0'); } }


        public Alu(IBus bus, IControlUnit controlUnit, IRegister aReg, IRegister bReg)
        {
            Bus = bus;

            this.aReg = aReg;
            this.bReg = bReg;

            busOutputLine = controlUnit.GetControlLine(ControlLineId.SUM_OUT);
            
            subLine = controlUnit.GetControlLine(ControlLineId.SUBTRACT);
            subLine.onTransition = () =>
            {
                // When the sub line changes, pull the value to refresh it, and the flags
                byte val = Value;
                return true;
            };


            busOutputLine.onTransition = () =>
            {
                if (busOutputLine.State == true)
                {
                    Bus.Driver = this;
                }
                else
                {
                    if (Bus.Driver == this)
                    {
                        Bus.Driver = null;
                    }
                }
                return true;
            };
        }


        public void OutputState(ValueFormat format)
        {
            Console.ForegroundColor = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ConsoleColor.Black : ConsoleColor.White;
            if (Bus.Driver == this)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y);
            Console.Write("|-----------------------|");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write("|                       |");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);

            switch(format)
            {
                case ValueFormat.Hex:
                Console.Write(String.Format("|ALU: 0x{0:X2} ", Value));
                break;

                case ValueFormat.Decimal:
                Console.Write(String.Format("|ALU: {0} ", Value));
                break;

                case ValueFormat.Binary:
                Console.Write(String.Format("|ALU: {0} ", BinaryValue));
                break;
            }

            if (Zero) Console.Write("Z");
            if (Carry) Console.Write("C");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }
    }

}
