using System;
using System.Drawing;

namespace Simulator
{

    public interface IAlu : IBusConnectedComponent
    {
        
        bool Carry { get; }
        bool Zero { get; }

        Point consoleXY { get; set; }
        void OutputState();
    }

}
