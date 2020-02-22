using System;
using System.Collections.Generic;
using System.Drawing;
using EightBitSystem;
using static Simulator.IClock;

namespace Simulator
{

    public class Clock : IClock
    {
        
        public Mode ClockMode { get; set; }

        public Point consoleXY { get; set; }
        public int FrequencyHz { get; set; }      
        public bool IsHalted { get { return hltLine.State;  } }

        ControlLine hltLine { get; set; }
        int cycleCount;
        

        public List<IClockConnectedComponent> clockConnectedComponents { get; private set; }

        public Clock(IControlUnit controlUnit)
        {
            //HltLine = controlUnit.GetControlLine(ControlLineId.HLT);
            FrequencyHz = 1;
            ClockMode = Mode.Stepped;

            clockConnectedComponents = new List<IClockConnectedComponent>();

            hltLine = controlUnit.GetControlLine(ControlLineId.HLT);
        }

        public void Step()
        {
            if(hltLine.State == true)
            {
                return;
            }

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
