using System;
using System.Drawing;
using System.IO;
using System.Threading;
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
        IRegister Flags { get; set; }
        Ram Ram { get; set; }
        Rom Rom { get; set; }
        IControlUnit ControlUnit { get; set; }
        ICounter ProgramCounter { get; set; }
        ICounter MicrostepCounter { get; set; }
        IBus Bus { get; set; }

        public EightBitSystem()
        {
            int leftPrint = 0;
            int rightPrint = 51;
            //Console.WindowHeight = 40;
            int moduleHeight = 3;

            this.ControlUnit = new ControlUnit();
            this.ControlUnit.consoleXY = new Point(leftPrint, 6 * moduleHeight);


            this.Clock = new Clock(this.ControlUnit);
            this.Clock.consoleXY = new Point(leftPrint, 0);

            this.Bus = new Bus();
            this.Bus.consoleXY = new Point(25, 0);

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
            this.Out.consoleXY = new Point(rightPrint, 5 * moduleHeight);

            this.ProgramCounter = new ProgramCounter(this.Clock, this.Bus, this.ControlUnit);
            this.ProgramCounter.consoleXY = new Point(rightPrint, 0);

            this.Alu = new Alu(this.ControlUnit, this.Bus, this.A, this.B);
            this.Alu.consoleXY = new Point(rightPrint, 2 * moduleHeight);

            this.Flags = new FlagsRegister4Bit(SystemRegister.FLAGS, this.Clock, this.ControlUnit, this.Alu);
            this.Flags.consoleXY = new Point(rightPrint, 4 * moduleHeight);

            this.MicrostepCounter = new MicrostepCounter(this.Clock, this.ControlUnit);

            // load the microcode images
            ControlUnit.LoadMicrocode();
            ControlUnit.InstructionRegister = this.Ir;
            ControlUnit.FlagsRegister = this.Flags;
            ControlUnit.MicrostepCounter = this.MicrostepCounter;
         
            string romFile = "../../../../Sample ASM/test.rom";
            MemoryStream romContents = new MemoryStream(File.ReadAllBytes(romFile));
            this.Rom = new Rom(this.Bus, this.ControlUnit, this.Mar);
            this.Rom.consoleXY = new Point(leftPrint, 2 * moduleHeight);
            this.Rom.Load(romContents);

            this.Ram = new Ram(this.Bus, this.ControlUnit, this.Mar);
            this.Ram.consoleXY = new Point(leftPrint, 3 * moduleHeight);

            // Start the computer with the control until poiting at the first control word
            this.ControlUnit.OnControlStateUpdated();

            ConsoleKeyInfo key;
            while (true)
            {
                // Left modules
                this.Clock.OutputState();
                Mar.OutputState();
                Ram.OutputState();
                Rom.OutputState();
                Ir.OutputState();
                IrParam.OutputState();

                // BUS
                this.Bus.OutputState();

                // Right modules
                ProgramCounter.OutputState();
                A.OutputState();
                Alu.OutputState();
                B.OutputState();
                Flags.OutputState();
                Out.OutputState();


                // Control Unit (bottom)
                ControlUnit.OutputState();

                Console.SetCursorPosition(0, 23);
                Console.Write(String.Format("[S]tep - [R]un - [N]ext Asm Instruction - Clock Freq {0}hz [+-] - E[x]it", this.Clock.FrequencyHz));


                if (this.Clock.ClockMode == IClock.Mode.Stepped ||
                    (this.Clock.ClockMode == IClock.Mode.Running && Console.KeyAvailable))
                {
                    key = Console.ReadKey(true);

                    int freq;

                    switch (key.Key)
                    {
                        case ConsoleKey.S:
                            if (this.Clock.ClockMode == IClock.Mode.Stepped)
                            {
                                this.Clock.Step();
                            }
                            else
                            {
                                this.Clock.ClockMode = IClock.Mode.Stepped;
                            }
                            break;

                        case ConsoleKey.R:
                            if (this.Clock.ClockMode != IClock.Mode.Running)                            
                            {
                                this.Clock.ClockMode = IClock.Mode.Running;
                            }
                            break;

                        case ConsoleKey.N:
                            this.Clock.Step();
                            while (this.ControlUnit.MicrostepCounter.Value != 3 && !this.Clock.IsHalted)
                            {
                                Thread.Sleep(1);
                                this.Clock.Step();
                            }
                            break;

                        case ConsoleKey.Add:
                            freq = this.Clock.FrequencyHz;
                            if (freq <= 20) freq++;
                            else if (freq <= 100) freq += 5;
                            else freq += 50;
                            if (freq > 1000) freq = 1000;
                            this.Clock.FrequencyHz = freq;
                            break;

                        case ConsoleKey.Subtract:
                            freq = this.Clock.FrequencyHz;
                            if (freq <= 20) freq--;
                            else if (freq <= 100) freq -= 5;
                            else freq -= 50;
                            if (freq < 1) freq = 1;
                            this.Clock.FrequencyHz = freq;
                            break;


                        case ConsoleKey.X:
                            return;
                    }

                    if (this.Clock.ClockMode == IClock.Mode.Stepped)
                    {
                        if (key.Key == ConsoleKey.S)
                        {

                        }
                    }
                    else if (this.Clock.ClockMode == IClock.Mode.Running)
                    {

                    }
                }

                if (this.Clock.ClockMode == IClock.Mode.Running)
                {
                    Thread.Sleep(1000 / this.Clock.FrequencyHz);
                    this.Clock.Step();
                }
            }
        }

            

        public void LoadProgram(string romFile)
        {

        }

        public void Reset()
        {

        }

    }

}
