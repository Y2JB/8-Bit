﻿using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using EightBitSystem;

namespace Simulator
{

    public class Ram : IMemoryController
    {
        // We use a 1K RAM chip
        private byte[] mem = new byte[1024];
        private IRegister mar;

        ControlLine busOutputLine;
        ControlLine busInputLine;

        public IBus Bus { get; private set; }
        public string Name { get { return "RAM"; } }    

        public byte Value { get { return Read(); } }

        public Point ConsoleXY { get; set; }


        public Ram(IBus bus, IControlUnit controlUnit, IRegister mar)
        {
            this.Bus = bus;
            busOutputLine = controlUnit.GetControlLine(ControlLineId.RAM_OUT);
            busInputLine = controlUnit.GetControlLine(ControlLineId.RAM_IN);

            this.mar = mar;

            // Setup the callback for when the bus output line goes high or low. Depending on which, we either start or stop driving the bus
            busOutputLine.onTransition = () =>
            {
                if (busOutputLine.State == true)
                {
                    Bus.Driver = this;
                }
                else
                {
                    if (Bus.Driver == this)
                    {
                        Bus.Driver = null;
                    }
                }
                return true;
            };

            // Setup the callback for when the bus output line goes high or low. Depending on which, we either start or stop driving the bus
            busInputLine.onTransition = () =>
            {
                if (busInputLine.State == true)
                {
                    Write(Bus.Value);
                }
                return true;
            };
        }


        public byte Read()
        {
            byte address = (byte) mar.Value;
            return mem[address];
        }


        public void Write(byte value)
        {
            byte address = (byte)mar.Value;
            mem[address] = value;
        }


        public void OutputState()
        {
            Console.ForegroundColor = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ConsoleColor.Black : ConsoleColor.White;
            if (Bus.Driver == this)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (busInputLine != null && busInputLine.State)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y);
            Console.Write("|-----------------------|");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write("|                       |");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write(String.Format("|RAM: 0x{0:X2}", Value));
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 2);
            Console.Write("|-----------------------|");
        }


    }

}
