using System;

namespace Simulator
{
    public interface IRegister
    {
        int Value { get; set; }
        string BinarytValue { get; }

        void SetBit(int bit, bool value);
        bool GetBit(int bit);
    }
}
