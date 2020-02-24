using System;
using System.Collections.Generic;
using System.Drawing;

namespace Simulator
{

    public interface IClock
    {
        public enum Mode
        {
            Running,
            Stepped
        }
        Mode ClockMode { get; set; }
        bool IsHalted { get; }
        int FrequencyHz { get; set; }

        void Step();

        List<IClockConnectedComponent> ClockConnectedComponents { get; }

        Point ConsoleXY { get; set; }
        void OutputState();
    }

}
