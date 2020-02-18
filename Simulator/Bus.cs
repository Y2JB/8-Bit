using System;

namespace Simulator
{

    // 8 bit Bus
    public class Bus : IBus
    {
        public byte Value()
        {
            return (driver == null) ? (byte) 0 : driver.Value;
        }
        
        public bool GetBit(int bit)
        {
            return false;
        }

        // Who is driving the bus?
        IBusConnectedComponent driver { get; set; }
    }

}
