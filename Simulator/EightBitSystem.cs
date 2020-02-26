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
        public IClock Clock { get; set; }
        public IAlu Alu { get; set; }
        public IRegister A { get; set; }
        public IRegister B { get; set; }
        public IRegister Out { get; set; }
        public IRegister Ir { get; set; }
        public IRegister IrParam { get; set; }
        public IRegister Mar { get; set; }
        public IRegister Flags { get; set; }
        public Ram Ram { get; set; }
        public Rom Rom { get; set; }
        public IControlUnit ControlUnit { get; set; }
        public ICounter ProgramCounter { get; set; }
        public ICounter MicrostepCounter { get; set; }
        public IBus Bus { get; set; }

        public EightBitSystem()
        {
            int leftPrint = 0;
            int rightPrint = 51;
            int moduleHeight = 3;

            this.ControlUnit = new ControlUnit();
            this.ControlUnit.ConsoleXY = new Point(leftPrint, 6 * moduleHeight);

            this.Clock = new Clock(this.ControlUnit);
            this.Clock.ConsoleXY = new Point(leftPrint, 0);

            this.Bus = new Bus();
            this.Bus.ConsoleXY = new Point(25, 0);

            this.A = new Register(SystemRegister.A, this.Clock, this.Bus, this.ControlUnit);
            this.A.ConsoleXY = new Point(rightPrint, 1 * moduleHeight);

            this.B = new Register(SystemRegister.B, this.Clock, this.Bus, this.ControlUnit);
            this.B.ConsoleXY = new Point(rightPrint, 3 * moduleHeight);

            this.Mar = new Register(SystemRegister.MAR, this.Clock, this.Bus, this.ControlUnit);
            this.Mar.ConsoleXY = new Point(leftPrint, 1 * moduleHeight);

            this.Ir = new Register(SystemRegister.IR, this.Clock, this.Bus, this.ControlUnit);
            this.Ir.ConsoleXY = new Point(leftPrint, 4 * moduleHeight);

            this.IrParam = new Register(SystemRegister.IR_PARAM, this.Clock, this.Bus, this.ControlUnit);
            this.IrParam.ConsoleXY = new Point(leftPrint, 5 * moduleHeight);

            this.Out = new Register(SystemRegister.OUT, this.Clock, this.Bus, this.ControlUnit);
            this.Out.ConsoleXY = new Point(rightPrint, 5 * moduleHeight);

            this.ProgramCounter = new ProgramCounter(this.Clock, this.Bus, this.ControlUnit);
            this.ProgramCounter.ConsoleXY = new Point(rightPrint, 0);

            this.Alu = new Alu(this.Bus, this.ControlUnit, this.A, this.B);
            this.Alu.ConsoleXY = new Point(rightPrint, 2 * moduleHeight);

            this.Flags = new FlagsRegister4Bit(SystemRegister.FLAGS, this.Clock, this.ControlUnit, this.Alu);
            this.Flags.ConsoleXY = new Point(rightPrint, 4 * moduleHeight);

            this.MicrostepCounter = new MicrostepCounter(this.Clock, this.ControlUnit);

            ControlUnit.InstructionRegister = this.Ir;
            ControlUnit.FlagsRegister = this.Flags;
            ControlUnit.MicrostepCounter = this.MicrostepCounter;
         
            this.Rom = new Rom(this.Bus, this.ControlUnit, this.Mar);
            this.Rom.ConsoleXY = new Point(leftPrint, 2 * moduleHeight);            

            this.Ram = new Ram(this.Bus, this.ControlUnit, this.Mar);
            this.Ram.ConsoleXY = new Point(leftPrint, 3 * moduleHeight);
        }


        public void LoadMicrocode(string bank0RomFile, string bank1RomFile, string bank2RomFile)
        {
            ControlUnit.LoadMicrocode(bank0RomFile, bank1RomFile, bank2RomFile);
        }


        public void LoadProgram(string romFile)
        {
            MemoryStream romContents = new MemoryStream(File.ReadAllBytes(romFile));
            this.Rom.Load(romContents);
        }


        public void PowerOn()
        {
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

                // User keys
                Console.SetCursorPosition(0, 23);
                Console.Write(String.Format("[S]tep - [N]ext Instruction - [R]un - Clock {0}hz [+-] - Rese[t] - E[x]it", this.Clock.FrequencyHz));


                // Step the system
                if ( this.Clock.ClockMode == IClock.Mode.Stepped ||
                    (this.Clock.ClockMode == IClock.Mode.Running && Console.KeyAvailable) )
                {
                    key = Console.ReadKey(true);

                    int freq;

                    switch (key.Key)
                    {
                        case ConsoleKey.S:
                            this.Clock.ClockMode = IClock.Mode.Stepped;
                            this.Clock.Step();
                            break;

                        case ConsoleKey.R:
                            this.Clock.ClockMode = IClock.Mode.Running;
                            break;

                        case ConsoleKey.N:
                            this.Clock.ClockMode = IClock.Mode.Stepped;
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
                            if (freq > 500) freq = 500;
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

                        case ConsoleKey.T:
                            Reset();
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
                    if(Clock.IsHalted)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Thread.Sleep(1000 / this.Clock.FrequencyHz);
                    }                    
                    this.Clock.Step();
                }
            }
        }


        public void Reset()
        {
            this.A.Reset();
            this.B.Reset();
            this.Mar.Reset();
            this.Ir.Reset();
            this.IrParam.Reset();
            this.Out.Reset();
            this.ProgramCounter.Reset();
            this.MicrostepCounter.Reset();

            this.ControlUnit.OnControlStateUpdated();
        }

    }

}
