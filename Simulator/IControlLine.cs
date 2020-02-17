using System;

namespace Simulator
{
    public abstract class IControlLine
    {
        Component ConnectionA { get; }
        Component ConnectionB { get; }
        bool State { get; }
    }
}
