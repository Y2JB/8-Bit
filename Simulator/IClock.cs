using System;
using System.Collections.Generic;

namespace Simulator
{

    public interface IClock
    {
        void Step();

        List<IClockConnectedComponent> clockConnectedComponents { get; }
    }

}
