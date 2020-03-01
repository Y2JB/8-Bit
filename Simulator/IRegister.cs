using System;
using System.Drawing;

namespace Simulator
{
    public interface IRegister : IClockConnectedComponent, IDisplayComponent
    {
        byte Value { get; }
        string BinaryValue { get; } 
        bool GetBit(int bit);
        void Reset();
    }
}
