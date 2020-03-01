using System;
using System.Drawing;

namespace Simulator
{

    public interface ICounter : IClockConnectedComponent, IDisplayComponent
    {
        bool CountEnabled { get; }
        byte MaxValue { get; }
        byte Value { get; }
        string BinaryValue { get; }
        void Reset();
    }

}
