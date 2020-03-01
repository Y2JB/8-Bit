using System;
using System.Drawing;

namespace Simulator
{
    public interface IDisplayComponent
    {
        enum ValueFormat
        {
            Hex,
            Decimal,
            Binary
        }

        Point ConsoleXY { get; set; }
        void OutputState(ValueFormat format);
    }
}
