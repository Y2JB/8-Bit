using System;

namespace Simulator
{

    public interface ICounter : IBusConnectedComponent
    {
        bool CountEnabled { get; set; }
        byte MaxValue { get; }

        void Load();
    }

}
