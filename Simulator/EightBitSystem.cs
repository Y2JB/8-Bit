using System;
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
            this.Clock = new Clock();
            this.ControlUnit = new ControlUnit(this.Clock);

            this.Bus = new Bus();
            
            this.A = new Register(SystemRegister.A, this.Clock, this.Bus, this.ControlUnit);
            this.B = new Register(SystemRegister.B, this.Clock, this.Bus, this.ControlUnit);
            this.Mar = new Register(SystemRegister.MAR, this.Clock, this.Bus, this.ControlUnit);
            this.Ir = new Register(SystemRegister.IR, this.Clock, this.Bus, this.ControlUnit);
            this.IrParam = new Register(SystemRegister.IR_PARAM, this.Clock, this.Bus, this.ControlUnit);
            this.Out = new Register(SystemRegister.OUT, this.Clock, this.Bus, this.ControlUnit);

            this.MicrostepCounter = new MicrostepCounter(this.Clock);

            // load the microcode images
            ControlUnit.LoadMicrocode();
            ControlUnit.InstructionRegister = this.Ir;
            ControlUnit.MicrostepCounter = this.MicrostepCounter;


            //string fn = "C:/Users/bellamj/source/repos/JonBellamy/8-Bit/Sample ASM/test.asm";
            string romFile = "/Users/jonbellamy/Projects/8-Bit/Sample ASM/test.rom";
            MemoryStream romContents = new MemoryStream(File.ReadAllBytes(romFile));
            this.Rom = new Rom(this.Bus, this.ControlUnit, this.Mar);
            this.Rom.Load(romContents);

            this.Ram = new Ram(this.Bus, this.ControlUnit, this.Mar);

            this.ProgramCounter = new ProgramCounter(this.Clock, this.Bus, this.ControlUnit);

            this.Alu = new Alu(this.ControlUnit, this.Bus, this.A, this.B);


            this.Clock.Step();
            this.Clock.Step();
            this.Clock.Step();
        }

            

        public void LoadProgram(string romFile)
        {

        }

        public void Reset()
        {

        }

    }

}
