using System;
using System.Collections.Generic;

namespace Simulator
{

    public interface IBusConnectedComponent
    {
        byte Value { get; }
        IBus Bus { get; }
    }

}
