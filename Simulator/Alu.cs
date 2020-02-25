using System;
using System.Drawing;
using System.Runtime.InteropServices;
using EightBitSystem;

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


        public void OutputState()
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
            Console.Write(String.Format("|ALU: 0x{0:X2} ", Value));
            if (Zero) Console.Write("Z");
            if (Carry) Console.Write("C");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }
    }

}
