using System;

namespace Simulator
{

    public interface IAlu : IBusConnectedComponent
    {
        
        bool Carry { get; }
        bool Zero { get; }
    }

}
