using EightBitSystem;
using System;

namespace Simulator
{
    // Control lines are brought high and low to drive the computer and make it perform useful operations
    // For example by bringing the RAM_OUT line high and the A_IN line high, then on the next clock cycle the contents of RAM point to by the MAR will go onto the bug
    // and theA register will latch and store the value from the bus 
    public class ControlLine
    {
        public ControlLineId Id { get; private set; }
        
        public bool State { get; set; }

        public ControlLine(ControlLineId id)
        {
            Id = id;
            State = false;
        }
    }
}
