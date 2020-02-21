using System;
using System.Drawing;

namespace Simulator
{

    public interface ICounter : IClockConnectedComponent
    {
        bool CountEnabled { get; }
        byte MaxValue { get; }
        byte Value { get; }

        Point consoleXY { get; set; }
        void OutputState();
    }

}
