using System;
using System.IO;

namespace asm
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string asmFile = "../../../../Simulator.IntegrationTest/Tests/CountToTen/test.asm";
            string romFile = "../../../../Simulator.IntegrationTest/Tests/CountToTen/test.rom";

            bool optionOutputPreProcessedSource = true;
            bool outputRomBinaryToConsole = true;

            Assembler asm = new Assembler();
            asm.Assemble(asmFile, romFile, optionOutputPreProcessedSource, outputRomBinaryToConsole);

            Console.WriteLine("done");            
        }
    }
}
