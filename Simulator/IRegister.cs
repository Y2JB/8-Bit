using System;
using System.Drawing;

namespace Simulator
{
    public interface IRegister : IClockConnectedComponent
    {
        byte Value { get; }
        string BinarytValue { get; } 
        bool GetBit(int bit);
        void Reset();
        Point ConsoleXY { get; set; }
        void OutputState();
    }
}
