using System;
using System.Drawing;

namespace Simulator
{
    public interface IRegister : IClockConnectedComponent
    {
        byte Value { get; }

        string BinarytValue { get; }

        void SetBit(int bit, bool value);
        bool GetBit(int bit);

        Point consoleXY { get; set; }
        void OutputState();
    }
}
