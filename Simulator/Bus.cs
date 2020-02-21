using System;
using System.Drawing;

namespace Simulator
{

    // 8 bit Bus
    public class Bus : IBus
    {
        public Point consoleXY { get; set; }

        public byte Value
        {
            get
            {
                return (Driver == null) ? (byte) 0 : Driver.Value;
            }
        }
        
        public bool GetBit(int bit)
        {
            return false;
        }

        // Who is driving the bus?
        public IBusConnectedComponent Driver { get; set; }


        public void OutputState()
        {
            Console.SetCursorPosition(consoleXY.X, consoleXY.Y);
            Console.Write(String.Format("     BUS - Value: {0}   ", Driver == null ? 0 : Driver.Value));

            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 1);
            Console.Write("                      ");

            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 1);
            Console.Write(String.Format("     Driver: {0}  ", Driver == null ? "null" : Driver.Name));

            Console.ForegroundColor = ConsoleColor.Black;
            if (Driver != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            for (int i=2; i < Console.WindowHeight - 5; i++)
            {
                Console.SetCursorPosition(consoleXY.X, consoleXY.Y + i);
                Console.Write("  |  |  |  |  |  |  |  |");
            }
        }
    }

}
