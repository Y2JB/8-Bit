using System;
using System.IO;


namespace Simulator
{

    public class Ram : IMemoryController
    {
        // We use a 1K RAM chip
        private MemoryStream mem = new MemoryStream(1024);
        private IRegister mar;

        public byte Value { get { return Read(); } }

        public Ram(IRegister mar)
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
            byte address = (byte)mar.Value;
            mem.GetBuffer()[address] = value;
        }

        public void Reset()
        {
        }
        public void OnClockPulse()
        {
        }

    }

}
