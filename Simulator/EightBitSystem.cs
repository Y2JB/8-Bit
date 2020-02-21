using System;
using System.Drawing;
using System.IO;
using EightBitSystem;

namespace Simulator
{

    // One thing to remember as you add features to this is that it needs to do what the hardware actually does, not what you wish the hardware could do!

    public class EightBitSystem
    {
        IClock Clock { get; set; }
        IAlu Alu { get; set; }
        IRegister A { get; set; }
        IRegister B { get; set; }        
        IRegister Out { get; set; }
        IRegister Ir { get; set; }
        IRegister IrParam { get; set; }        
        IRegister Mar { get; set; }
        //IRegister Flags { get; set; }
        Ram Ram { get; set; }
        Rom Rom { get; set; }
        IControlUnit ControlUnit { get; set; }
        ICounter ProgramCounter { get; set; }
        ICounter MicrostepCounter { get; set; }
        IBus Bus { get; set; }

        public EightBitSystem()
        {
            int leftPrint = 0;
            int rightPrint = Console.WindowWidth - 30;
            int consoleHeight = Console.WindowHeight;
            //Console.WindowHeight = 40;
            int moduleHeight = 3;

            this.Clock = new Clock();

            this.ControlUnit = new ControlUnit(this.Clock);
            this.ControlUnit.consoleXY = new Point(leftPrint, 6 * moduleHeight);


            this.Bus = new Bus();          


            this.A = new Register(SystemRegister.A, this.Clock, this.Bus, this.ControlUnit);
            this.A.consoleXY = new Point(rightPrint, 1 * moduleHeight);

            this.B = new Register(SystemRegister.B, this.Clock, this.Bus, this.ControlUnit);
            this.B.consoleXY = new Point(rightPrint, 3 * moduleHeight);

            this.Mar = new Register(SystemRegister.MAR, this.Clock, this.Bus, this.ControlUnit);
            this.Mar.consoleXY = new Point(leftPrint, 1 * moduleHeight);

            this.Ir = new Register(SystemRegister.IR, this.Clock, this.Bus, this.ControlUnit);
            this.Ir.consoleXY = new Point(leftPrint, 4 * moduleHeight);

            this.IrParam = new Register(SystemRegister.IR_PARAM, this.Clock, this.Bus, this.ControlUnit);
            this.IrParam.consoleXY = new Point(leftPrint, 5 * moduleHeight);

            this.Out = new Register(SystemRegister.OUT, this.Clock, this.Bus, this.ControlUnit);
            this.Out.consoleXY = new Point(rightPrint, 4 * moduleHeight);

            this.MicrostepCounter = new MicrostepCounter(this.Clock);

            // load the microcode images
            ControlUnit.LoadMicrocode();
            ControlUnit.InstructionRegister = this.Ir;
            ControlUnit.MicrostepCounter = this.MicrostepCounter;


            //string fn = "C:/Users/bellamj/source/repos/JonBellamy/8-Bit/Sample ASM/test.asm";
            string romFile = "/Users/jonbellamy/Projects/8-Bit/Sample ASM/test.rom";
            MemoryStream romContents = new MemoryStream(File.ReadAllBytes(romFile));
            this.Rom = new Rom(this.Bus, this.ControlUnit, this.Mar);
            this.Rom.consoleXY = new Point(leftPrint, 2 * moduleHeight);
            this.Rom.Load(romContents);

            this.Ram = new Ram(this.Bus, this.ControlUnit, this.Mar);
            this.Ram.consoleXY = new Point(leftPrint, 3 * moduleHeight);

            this.ProgramCounter = new ProgramCounter(this.Clock, this.Bus, this.ControlUnit);
            this.ProgramCounter.consoleXY = new Point(rightPrint, 0);

            this.Alu = new Alu(this.ControlUnit, this.Bus, this.A, this.B);
            this.Alu.consoleXY = new Point(rightPrint, 2 * moduleHeight);

            ConsoleKeyInfo key;


            do
            {
                // Left modules
                Mar.OutputState();
                Ram.OutputState();
                Rom.OutputState();
                Ir.OutputState();
                IrParam.OutputState();

                // TODO BUS

                // Right modules
                A.OutputState();
                Alu.OutputState();
                B.OutputState();
                ProgramCounter.OutputState();
                Out.OutputState();


                // Control Unit (bottom)
                ControlUnit.OutputState();

                Console.SetCursorPosition(0, 23);
                Console.Write("[S]tep [R]un E[x]it");

                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.S)
                {
                    this.Clock.Step();
                }
            }
            while (key.Key != ConsoleKey.X);
        }

            

        public void LoadProgram(string romFile)
        {

        }

        public void Reset()
        {

        }

    }

}
