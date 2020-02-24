using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Simulator
{

    // 8 bit Bus
    public class Bus : IBus
    {
        public Point ConsoleXY { get; set; }

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
            Console.ForegroundColor = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ConsoleColor.Black : ConsoleColor.White;

            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y);
            Console.Write(String.Format("     BUS - Value: 0x{0:X2}   ", Driver == null ? 0 : Driver.Value));

            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write("                      ");

            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write(String.Format("     Driver: {0}  ", Driver == null ? "null" : Driver.Name));

            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 2);
            Console.Write(String.Format("  7  6  5  4  3  2  1  0", Driver == null ? "null" : Driver.Name));

            for (int i=3; i < 17; i++)
            {
                Console.SetCursorPosition(ConsoleXY.X + 2, ConsoleXY.Y + i);
                for (int bit = 7; bit >= 0; bit--)
                {
                    Console.ForegroundColor = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ConsoleColor.Black : ConsoleColor.White;
                    // Draw active lines in red
                    if (Driver != null && GetBit(bit))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write("|  ");
                }
            }
        }
    }

}
