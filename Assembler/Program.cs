using System;
using System.IO;

namespace asm
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string fn = "/Users/jonbellamy/Projects/8-Bit/Sample ASM/test.asm";
            //string fn = "C:/Users/bellamj/source/repos/JonBellamy/8-Bit/Sample ASM/test.asm";


            bool optionOutputPreProcessedSource = true;
            bool outputRomBinaryToConsole = true;

            Assembler asm = new Assembler();
            asm.Assemble(fn, "fib.rom", optionOutputPreProcessedSource, outputRomBinaryToConsole);

            Console.WriteLine("done");

            
        }
    }
}
