using System;
using System.IO;

namespace asm
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string asmFile = "../../../../Sample ASM/test.asm";
            string romFile = "../../../../Sample ASM/test.rom";

            bool optionOutputPreProcessedSource = true;
            bool outputRomBinaryToConsole = true;

            Assembler asm = new Assembler();
            asm.Assemble(asmFile, romFile, optionOutputPreProcessedSource, outputRomBinaryToConsole);

            Console.WriteLine("done");            
        }
    }
}
