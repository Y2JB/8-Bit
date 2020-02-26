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
        int CycleCount { get; }

        void Step();

        void AddConnectedComponent(IClockConnectedComponent component);
        
        Point ConsoleXY { get; set; }
        void OutputState();
    }

}
