using System;
using System.Drawing;

namespace Simulator
{

    // 8 bit Bus
    public class Bus : IBus
    {
        public Point consoleXY { get; set; }

        // Who is driving the bus?
        public IBusConnectedComponent Driver { get; set; }

        public byte Value
        {
            get
            {
                return (Driver == null) ? (byte) 0 : Driver.Value;
            }
        }


        public bool GetBit(int bit)
        {
            if (Driver == null) return false;

            if (bit < 0 || bit > 7)
            {
                throw new ArgumentException("Bit must be 0 - 7");
            }

            int mask = (byte)(1 << bit);
            return (Driver.Value & mask) != 0; 
        }
    

        public void OutputState()
        {
            Console.ForegroundColor = ConsoleColor.Black;

            Console.SetCursorPosition(consoleXY.X, consoleXY.Y);
            Console.Write(String.Format("     BUS - Value: {0}   ", Driver == null ? 0 : Driver.Value));

            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 1);
            Console.Write("                      ");

            Console.SetCursorPosition(consoleXY.X, consoleXY.Y + 1);
            Console.Write(String.Format("     Driver: {0}  ", Driver == null ? "null" : Driver.Name));

            if (Driver != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            for (int i=2; i < Console.WindowHeight - 6; i++)
            {
                Console.SetCursorPosition(consoleXY.X, consoleXY.Y + i);
                Console.Write("  |  |  |  |  |  |  |  |");
            }
        }
    }

}
