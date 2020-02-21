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

        public bool Carry { get { return false; } }
        public bool Zero { get { return false; } }

        ControlLine busOutputLine;
        ControlLine subLine;

        public Point consoleXY { get; set; }

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
            Console.Write(String.Format("|ALU - 0x{0:X2} Z {1} C {2}", Value, Zero, Carry));
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }
    }

}
