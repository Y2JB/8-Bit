using System;
using System.Drawing;
using System.IO;
using EightBitSystem;

namespace Simulator
{

    public class Rom : IMemoryController
    {
        // We use a 256 kbit ROM chip
        private MemoryStream mem = new MemoryStream(32 * 1024);
        private IRegister mar;

        ControlLine busOutputLine;
        ControlLine romBank1Line;

        public IBus Bus { get; private set; }

        public byte Value { get { return Read(); } }

        public Point consoleXY { get; set; }


        public Rom(IBus bus, IControlUnit controlUnit, IRegister mar)
        {
            this.Bus = bus;
            busOutputLine = controlUnit.GetControlLine(ControlLineId.ROM_OUT);
            romBank1Line = controlUnit.GetControlLine(ControlLineId.ROM_BANK_1);
            this.mar = mar;

            // Setup the callback for when the bus output line goes high or low. Depending on which, we either start or stop driving the bus
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

        // Used to load code
        public void Load(MemoryStream eepromContents)
        {
            eepromContents.CopyTo(mem);
        }

        public byte Read()
        {
            int address = (int) mar.Value;
            if(romBank1Line.State)
            {
                address = address | 0x10;
            }
            return mem.GetBuffer()[address];
        }


        public void Write(byte value)
        {
            throw new InvalidOperationException();
        }

        public void OutputState()
        {
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y);
            Console.Write("|-----------------------|");
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 1);
            Console.Write(String.Format("ROM - {0}", Value));
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }
    }

}
