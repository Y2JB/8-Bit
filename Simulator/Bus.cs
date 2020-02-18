using System;

namespace Simulator
{

    // 8 bit Bus
    public class Bus : IBus
    {
        public byte Value
        {
            get
            {
                return (Driver == null) ? (byte) 0 : Driver.Value;
            }
        }
        
        public bool GetBit(int bit)
        {
            return false;
        }

        // Who is driving the bus?
        public IBusConnectedComponent Driver { get; set; }
    }

}
