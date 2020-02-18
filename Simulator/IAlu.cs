using System;

namespace Simulator
{

    public interface IAlu : IBusConnectedComponent
    {
        // Add / Sub A to/from B
        int Add();
        int Sub();

        bool Carry { get; }
        bool Zero { get; }
    }

}
