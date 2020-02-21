using System;
using System.Collections.Generic;

namespace Simulator
{

    public interface IBusConnectedComponent
    {
        public string Name { get; }
        byte Value { get; }
        IBus Bus { get; }
    }

}
