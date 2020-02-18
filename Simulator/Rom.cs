using System;
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

        public IBus Bus { get; private set; }

        public byte Value { get { return Read(); } }


        public Rom(IBus bus, IControlUnit controlUnit, IRegister mar)
        {
            this.Bus = bus;
            busOutputLine = controlUnit.GetControlLine(ControlLineId.ROM_OUT);
            this.mar = mar;
        }

        // Used to load code
        public void Load(MemoryStream eepromContents)
        {
            eepromContents.CopyTo(mem);
        }

        public byte Read()
        {
            byte address = (byte) mar.Value;
            return mem.GetBuffer()[address];
        }


        public void Write(byte value)
        {
            throw new InvalidOperationException();
        }

    }

}
