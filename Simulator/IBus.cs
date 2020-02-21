using System;
using System.Drawing;

namespace Simulator
{

    // 8 bit Bus
    public interface IBus
    {
        byte Value { get; }
        
        bool GetBit(int bit);

        // Who is driving the bus?
        IBusConnectedComponent Driver { get; set; }

        Point consoleXY { get; set; }
        void OutputState();
    }

}
