using EightBitSystem;
using System;

namespace Simulator
{
    public abstract class IControlLine
    {
        ControlLineId Id { get; }

        string Name { get; }
        
        bool State { get; }
    }
}
