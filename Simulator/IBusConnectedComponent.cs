using System;
using System.Collections.Generic;

namespace Simulator
{

    public interface IBusConnectedComponent
    {
        void Reset();
        void OnClockPulse();

        byte Value { get; }
    }

}
