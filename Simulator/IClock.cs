using System;

namespace Simulator
{

    public interface IClock
    {
        void Step();

        void OnHigh();
        void OnLow();
    }

}
