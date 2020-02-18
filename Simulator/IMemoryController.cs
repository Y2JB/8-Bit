using System;

namespace Simulator
{

    public interface IMemoryController : IBusConnectedComponent
    {
        // NB: We do not pass the address as this comes from the Memory Address Register (MAR)

        byte Read();
        void Write(byte value);
    }

}
