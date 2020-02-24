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

        public Point ConsoleXY { get; set; }
        public int FrequencyHz { get; set; }      
        public bool IsHalted { get { return HltLine.State;  } }

        ControlLine HltLine { get; set; }
        int cycleCount;
        

        public List<IClockConnectedComponent> ClockConnectedComponents { get; private set; }

        public Clock(IControlUnit controlUnit)
        {
            //HltLine = controlUnit.GetControlLine(ControlLineId.HLT);
            FrequencyHz = 1;
            ClockMode = Mode.Stepped;

            ClockConnectedComponents = new List<IClockConnectedComponent>();

            HltLine = controlUnit.GetControlLine(ControlLineId.HLT);
        }

        public void Step()
        {
            if(HltLine.State == true)
            {
                return;
            }

            cycleCount++;

            foreach(IClockConnectedComponent component in ClockConnectedComponents)
            {
                component.OnRisingEdge();
            }

            foreach (IClockConnectedComponent component in ClockConnectedComponents)
            {
                component.OnFallingEdge();
            }
        }



        public void OutputState()
        {
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y);
            Console.Write("|-----------------------|");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write("|                       |");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write(String.Format("|Clock - Cycle: {0}", cycleCount));
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }

    }

}
