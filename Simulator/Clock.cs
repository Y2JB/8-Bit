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
        public int CycleCount { get; private set; }

        ControlLine HltLine { get; set; }
        

        List<IClockConnectedComponent> clockConnectedComponents;

        public Clock(IControlUnit controlUnit)
        {
            //HltLine = controlUnit.GetControlLine(ControlLineId.HLT);
            FrequencyHz = 1;
            ClockMode = Mode.Stepped;

            clockConnectedComponents = new List<IClockConnectedComponent>();

            HltLine = controlUnit.GetControlLine(ControlLineId.HLT);
        }


        public void AddConnectedComponent(IClockConnectedComponent component)
        {
            clockConnectedComponents.Add(component);
        }


        public void Step()
        {
            if(HltLine.State == true)
            {
                return;
            }

            CycleCount++;

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
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y);
            Console.Write("|-----------------------|");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write("|                       |");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write(String.Format("|Clock - Cycle: {0}", CycleCount));
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }

    }

}
