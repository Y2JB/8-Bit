using System;
using System.Collections.Generic;

namespace Simulator
{

    public interface IClockConnectedComponent
    {
        void OnRisingEdge();
        void OnFallingEdge();
    }

}
