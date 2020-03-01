using System;
using System.Drawing;

namespace Simulator
{

    public interface IAlu : IBusConnectedComponent, IDisplayComponent
    {        
        bool Carry { get; }
        bool Zero { get; }
    }

}
