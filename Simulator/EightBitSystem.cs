using System;
using System.IO;


namespace Simulator
{

    // One thing to remember as you add features to this is that it needs to do what the hardware actually does, not what you wish the hardware could do!

    public class EightBitSystem
    {
        IClock Clock { get; }
        IAlu Alu { get; }
        IRegister A { get; }
        IRegister B { get; }        
        IRegister Out { get; }
        IRegister Ir { get; }
        IRegister IrParam { get; }        
        IRegister Mar { get; }
        IMemoryController Ram { get; }
        IMemoryController Rom { get; }
        IControlUnit ControlUnit { get; }
        ICounter ProgramCounter { get; }
        IBus Bus { get; }

        public EightBitSystem()
        {
            // load the microcodeimages
            ControlUnit.LoadMicrocode(null, null, null);
        }

            

        public void LoadProgram(string romFile)
        {

        }

        public void Reset()
        {

        }

    }

}
