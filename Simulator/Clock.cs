using System;
using System.Collections.Generic;
using System.Drawing;
using EightBitSystem;

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

        public Point consoleXY { get; set; }

        int frequencyHz;
        int cycleCount;
        Mode mode;
        //ControlLine HltLine;

        public List<IClockConnectedComponent> clockConnectedComponents { get; private set; }

        public Clock()
        {
            //HltLine = controlUnit.GetControlLine(ControlLineId.HLT);
            frequencyHz = 1;
            mode = Mode.Halted;

            clockConnectedComponents = new List<IClockConnectedComponent>();
        }

        public void Step()
        {
            cycleCount++;

            foreach(IClockConnectedComponent component in clockConnectedComponents)
            {
                component.OnRisingEdge();
            }

            foreach (IClockConnectedComponent component in clockConnectedComponents)
            {
                component.OnFallingEdge();
            }
        }



        public void OutputState()
        {
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y);
            Console.Write("|-----------------------|");
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 1);
            Console.Write("|                       |");
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 1);
            Console.Write(String.Format("|Clock. Cycle - {0}", cycleCount));
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }

    }

}
