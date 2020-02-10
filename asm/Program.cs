using System;
using System.IO;

namespace asm
{
    class Program
    {
        
        static void Main(string[] args)
        {
            ///string fn = "/Users/jonbellamy/Google Drive/Electronics/8 Bit CPU/asm/Sample ASM/win.asm.txt";
            string fn = "C:/Users/bellamj/Google Drive/Electronics/8 Bit CPU/asm/Sample ASM/win.asm.txt";
       // C: \Users\Jon\Google Drive\Electronics\8 Bit CPU\asm\Sample ASM

            bool optionOutputPreProcessedSource = true;
            bool outputRomBinaryToConsole = true;

            Assembler asm = new Assembler();
            asm.Assemble(fn, "fib.rom", optionOutputPreProcessedSource, outputRomBinaryToConsole);

            Console.WriteLine("done");

            
        }
    }
}
