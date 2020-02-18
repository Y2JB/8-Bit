using System;

namespace Simulator
{

    public interface ICounter : IBusConnectedComponent, IClockConnectedComponent
    {
        bool CountEnabled { get; }
        byte MaxValue { get; }
    }

}
