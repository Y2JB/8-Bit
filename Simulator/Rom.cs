using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using EightBitSystem;
using static Simulator.IDisplayComponent;

namespace Simulator
{

    public class Rom : IMemoryController
    {
        // We use a 256 kbit ROM chip
        private byte[] mem = new byte[32 * 1024];
        private IRegister mar;

        ControlLine busOutputLine;
        ControlLine romBank1Line;

        public IBus Bus { get; private set; }
        public string Name { get { return "ROM"; } }

        public byte Value { get { return Read(); } }
        public string BinaryValue { get { return Convert.ToString(Value, 2).PadLeft(8, '0'); } }

        public Point ConsoleXY { get; set; }


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
            mem = eepromContents.ToArray();
        }


        public byte Read()
        {
            int address = (int) mar.Value;
            if(romBank1Line.State)
            {
                address |= 0x100;
            }
            byte value = mem[address];
            return value;
        }


        public void Write(byte value)
        {
            throw new InvalidOperationException();
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

            switch (format)
            {
                case ValueFormat.Hex:
                    Console.Write(String.Format("|ROM: 0x{0:X2}", Value));
                    break;

                case ValueFormat.Decimal:
                    Console.Write(String.Format("|ROM: {0}", Value));
                    break;

                case ValueFormat.Binary:
                    Console.Write(String.Format("|ROM: {0}", BinaryValue));
                    break;
            }
            
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }
    }

}
