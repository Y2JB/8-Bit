using System;
using System.Collections.Generic;

namespace Simulator
{

    public interface IBusConnectedComponent
    {
        byte Value { get; }
        string Name { get; }
        IBus Bus { get; }
    }

}
