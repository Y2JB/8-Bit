using System;

namespace Simulator
{

    public interface ICounter : IClockConnectedComponent
    {
        bool CountEnabled { get; }
        byte MaxValue { get; }
        byte Value { get; }
    }

}
