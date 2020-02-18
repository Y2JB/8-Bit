using System;

namespace Simulator
{

    // 8 bit Bus
    public interface IBus
    {
        byte Value();
        
        bool GetBit(int bit);
    }

}
