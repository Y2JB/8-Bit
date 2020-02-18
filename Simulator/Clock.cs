using System;

namespace Simulator
{

    public class Clock : IClock
    {
        public enum Mode
        {
            Halted,
            Running,
            Stepped
        }

        int frequencyHz;
        Mode mode;
        IControlLine HltLine;


        public Clock()
        {
            frequencyHz = 1;
            mode = Mode.Halted;
        }

        public void Step()
        {

        }

        public void OnHigh()
        {

        }

        public void OnLow()
        {

        }
    }

}
