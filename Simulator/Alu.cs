using System;

namespace Simulator
{

    public class Alu : IAlu
    {
        public bool Carry { get { return false; } }
        public bool Zero { get { return false; } }

        public byte Value { get; }

        public IControlLine SubControlLine { get; }


        public Alu()
        {
        }

        // Add / Sub A to/from B
        public int Add()
        {
            return 0;
        }
        public int Sub()
        {
            return 0;
        }

        public void Reset()
        {
        }

        public void OnClockPulse()
        {
        }
    }

}
