using System;

namespace Simulator
{
    public interface IRegister : IBusConnectedComponent, IClockConnectedComponent
    {
        string BinarytValue { get; }

        void SetBit(int bit, bool value);
        bool GetBit(int bit);
    }
}
