using EightBitSystem;
using System;

namespace Simulator
{
    public abstract class IControlLine
    {
        ControlLineId Id { get; }

        string Name { get; }

        IBusConnectedComponent ConnectionA { get; }
        IBusConnectedComponent ConnectionB { get; }
        
        bool State { get; }
    }
}
