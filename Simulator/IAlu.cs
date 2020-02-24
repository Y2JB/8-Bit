using System;
using System.Drawing;

namespace Simulator
{

    public interface IAlu : IBusConnectedComponent
    {
        
        bool Carry { get; }
        bool Zero { get; }

        Point ConsoleXY { get; set; }
        void OutputState();
    }

}
