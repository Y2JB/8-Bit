using System;
using System.Drawing;
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

        public Point consoleXY { get; set; }

        public byte Value
        {
            get
            {
                int val;
                if (subLine.State == true)
                {
                    val = (byte) (aReg.Value - bReg.Value);
                }
                else
                {
                    val = (byte) (aReg.Value + bReg.Value);
                }
                Zero = (val == 0);
                Carry = (val > 255 || val < -127);
               
                return (byte) val;  
            }
        }

        public Alu(IControlUnit controlUnit, IBus bus, IRegister aReg, IRegister bReg)
        {
            Bus = bus;

            this.aReg = aReg;
            this.bReg = bReg;

            busOutputLine = controlUnit.GetControlLine(ControlLineId.SUM_OUT);
            subLine = controlUnit.GetControlLine(ControlLineId.SUBTRACT);

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
            Console.ForegroundColor = ConsoleColor.Black;
            if (Bus.Driver == this)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.SetCursorPosition(consoleXY.X, consoleXY.Y);
            Console.Write("|-----------------------|");
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 1);
            Console.Write("|                       |");
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 1);
            Console.Write(String.Format("|ALU - 0x{0:X2} ", Value));
            if (Zero) Console.Write("Z");
            if (Carry) Console.Write("C");
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }
    }

}
