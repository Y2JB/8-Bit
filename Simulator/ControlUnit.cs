using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using EightBitSystem;

namespace Simulator
{

    public class ControlUnit : IControlUnit
    {
        public IRegister InstructionRegister { get;  set; }
        public IRegister FlagsRegister { get; set; }
        public ICounter MicrostepCounter { get; set; }

        Dictionary<ControlLineId, ControlLine> controlLines = new Dictionary<ControlLineId, ControlLine>();
        byte[] microcodeEeprom0;
        byte[] microcodeEeprom1;
        byte[] microcodeEeprom2;

        UInt16 microcodeAddress;

        public Point ConsoleXY { get; set; }

        public ControlUnit()
        {           
            CreateControlLines();
        }


        public void LoadMicrocode(string bank0RomFile, string bank1RomFile, string bank2RomFile)
        {
            microcodeEeprom0 = new MemoryStream(File.ReadAllBytes("../../../../Sample Microcode/Microcode-Bank0.bin")).ToArray();
            microcodeEeprom1 = new MemoryStream(File.ReadAllBytes("../../../../Sample Microcode/Microcode-Bank1.bin")).ToArray();
            microcodeEeprom2 = new MemoryStream(File.ReadAllBytes("../../../../Sample Microcode/Microcode-Bank2.bin")).ToArray();
        }


        void CreateControlLines()
        {
            foreach (ControlLineId lineId in Enum.GetValues(typeof(ControlLineId)))
            {
                controlLines.Add(lineId, new ControlLine(lineId));
            }
        }


        public ControlLine GetControlLine(ControlLineId lineId)
        {
            return controlLines[lineId];
        }


        // Our control address / lookup is based on 15 bits the Instruction and Register it operateson (8 bits) the flags (4 bits) and the microstep (3 bits).
        // If any one of them changes we call this to immediately update the control unit. In hardware the control unit is not directly connected to the clock, it is
        // just hardwired to the 3 address inputs.
        public void OnControlStateUpdated()
        {
            int microStep = MicrostepCounter.Value;
            int flags = FlagsRegister.Value;

            microcodeAddress = (UInt16)(((UInt16)(InstructionRegister.Value) << 7) | flags << 3 | microStep);

            UInt32 controlWord = (UInt32) ((microcodeEeprom2[microcodeAddress] << 16) | (microcodeEeprom1[microcodeAddress] << 8) | microcodeEeprom0[microcodeAddress]);

            foreach (var line in controlLines)
            {
                bool newLineValue = false;
                UInt32 lineid = (UInt32)(line.Key);
                if ((controlWord & lineid) != 0)
                {
                    newLineValue = true;
                }

                if(line.Value.State != newLineValue)
                {
                    line.Value.State = newLineValue;
                    if (line.Value.onTransition != null) line.Value.onTransition();
                }

            }
        }


        public void OutputState()
        {
            ConsoleColor defaultColour = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ConsoleColor.Black : ConsoleColor.White;
            Console.ForegroundColor = defaultColour;
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y);
            Console.Write("|--------------------------------------------------------------------------|");
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 1);
            Console.Write("|                                                                          |");
            Console.SetCursorPosition(ConsoleXY.X + 1, ConsoleXY.Y + 1);
            Console.Write(String.Format("MicroStep: {0} - Microcode Address {1}", MicrostepCounter.Value, microcodeAddress));
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 2);
            Console.Write("|                                                                          |");
            Console.SetCursorPosition(ConsoleXY.X + 1, ConsoleXY.Y + 2);            
            foreach (var line in controlLines)
            {
                if (line.Value.State)
                {
                    Console.Write(line.Key.ToString() + " ");
                }
            }
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 3);
            Console.Write("|                                                                          |");
            Console.SetCursorPosition(ConsoleXY.X + 1, ConsoleXY.Y + 3);
            foreach (var line in controlLines) 
            {
                Console.ForegroundColor = line.Value.State ? ConsoleColor.Red : defaultColour;
                Console.Write("X ");

            }
            Console.ForegroundColor = defaultColour;
            Console.SetCursorPosition(ConsoleXY.X, ConsoleXY.Y + 4);
            Console.Write("|--------------------------------------------------------------------------|");

        }
    }

}
