using System;
using System.IO;


namespace Simulator
{

    public class Rom : IMemoryController
    {
        // We use a 256 kbit ROM chip
        private MemoryStream mem = new MemoryStream(32 * 1024);
        private IRegister mar;

        IControlLine busOutputLine;

        public byte Value { get { return Read(); } }


        public Rom(IRegister mar)
        {
            this.mar = mar;
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

        public void Reset()
        {
        }

        public void OnClockPulse()
        {
        }

    }

}
