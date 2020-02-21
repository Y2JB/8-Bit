using System;
using System.Collections.Generic;
using System.Drawing;

namespace Simulator
{

    public interface IClock
    {
        void Step();

        List<IClockConnectedComponent> clockConnectedComponents { get; }

        Point consoleXY { get; set; }
        void OutputState();
    }

}
