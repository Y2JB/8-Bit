using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using EightBitSystem;

namespace asm
{
    // Assembler for an 8 Bit computer with the following specs:
    // 8 Bit Registers
    // 8 Bit Program Counter
    // 8 Bit Bus
    // 5 Bit Op Codes
    // 3 Bit LParam
    // 8 Bit R Param
    // The output ROM format is so:
    // Op    Lparam
    // XXXXX XXX
    // ..... ...
    // XXXXX XXX
    // Rparam
    // XXXXXXXX
    // ........
    // The 8 bit r parmas for each instructions come after all of the op code and lparams.
    // This is so that the 8 bit Program Counter can advance in increments of 1 and the 16 bits of instruction data can still be fetched using only an 8 bit PC
    // The fetch cycle therefore reads instruction 4 (for example) and pulls the first byte from memory address 4 to fetch the op code and lparam. It then
    // does a second fetch and pulls byte 4 again but with a bit 9 set high (so address 260) and fetched the r param.
    public class Assembler
    {
        public Assembler()
        {
            
        }

        private Dictionary<string, int> Labels = new Dictionary<string, int>();
        private List<Statement> Program = new List<Statement>();


        public void Assemble(string sourceFileName, string outputRomFile, bool optionOutputPreProcessedSource, bool outputRomBinaryToConsole)
        {
            // Read source
            string[] sourceLines = File.ReadAllLines(sourceFileName);


            // Keep the original source and (importantly for error reporting) the original 1 based line numbers 
            var sourceCode = new List<Tuple<int, string>>();
            for (int i=0; i < sourceLines.Length; i++)
            {
                sourceCode.Add(new Tuple<int, string>(i+1, sourceLines[i]));                                   
            }

            // Preprocessor 
            List<Tuple<int, string>> preprocessedSource = PreProcess(sourceCode);

            if(optionOutputPreProcessedSource)
            {
                Console.WriteLine("Writing Pre-processed source file.");
                using (var stream = File.CreateText(sourceFileName + ".pre"))
                {
                    foreach (var line in preprocessedSource)
                    {
                        stream.WriteLine(line.Item2);
                    }
                }
            }

            // Parse and Validate            
            foreach(var line in preprocessedSource)
            {
                try
                {
                    Statement statement = new Statement(line.Item1, line.Item2);
                    Program.Add(statement);

                    if(Program.Count > 256)
                    {
                        throw new Exception("Program exceeds max size of 256 instructions.");
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Failed to Assemble.");
                    Console.WriteLine(e.Message);
                    Environment.Exit(0);
                }
            }
            Console.WriteLine(String.Format("Program contains {0} statements", Program.Count));

            // Write executable ROM image
            WriteROM(outputRomFile, outputRomBinaryToConsole);


            Console.WriteLine("\nAssembled OK!");
        }


        private void WriteROM(string outputRomFile, bool outputRomBinaryToConsole)
        {
            Console.Write("Writing ROM...");

            // When the computer runs, the program counter has an address space of 0 - 255
            // When it wants to load instruction 4 it loads the byte at address 4 into the 8 bit Instruction Register, this loads the op code and lparm (4 bits each)
            // The fetch cycle then does a second read for instruction 4 but sets bit 8 (the 9th bit) of the memory controller address line high. This loads a byte from address 300. 
            // This load will contain instruction 4's 8 bit right paramter. 

            // // To keep things simple we will make the ROM as large as it can be. Each instructiom is two bytes wide so 256 * 2 = 512
            var memStream = new MemoryStream(512);

            // First section of the ROM stores our 4 bit instruction and 4 bit left parameter
            foreach (var statement in Program)
            {
                byte op = statement.Instruction.Value;
                byte lp = statement.LeftParam.Value;
                byte opcodeAndLParam = (byte)((op << 3) | lp);
                memStream.WriteByte(opcodeAndLParam);
            }

            // Second section of the ROM contains the 8 bit right parameter
            memStream.Position = 256;
            foreach (var statement in Program)
            {
                byte rp = statement.RightParam.Value;
                memStream.WriteByte(rp);
            }

                     
            using (FileStream f = File.Create(outputRomFile))
            {
                f.Write(memStream.GetBuffer());
                f.Close();
            }

            Console.WriteLine("success");

            if (outputRomBinaryToConsole)
            {
                OutputRomToConsole(memStream);
            }
        }


        private void OutputRomToConsole(MemoryStream memStream)
        {
            Console.WriteLine("Logical ROM contents");
            Console.WriteLine("#     Op      LP    RP");
            for (int i = 0; i < Program.Count; i++)
            {
                // Shift off the 3 bits LParam
                byte op = (byte) (memStream.GetBuffer()[i] >> 3);
                // Mask out the 5 bits op code
                byte lp = (byte) (memStream.GetBuffer()[i] & 0x07);
                byte rp = memStream.GetBuffer()[i + 256];
                Console.WriteLine("{0:X2}    {1}   {2}   {3} ", i, Convert.ToString(op, 2).PadLeft(5, '0'), Convert.ToString(lp, 2).PadLeft(3, '0'), Convert.ToString(rp, 2).PadLeft(8, '0'));
            }
        }


        // Removes all comments and unnecessary whitespace. 
        private List<Tuple<int, string>> PreProcess(List<Tuple<int, string>> source)
        {
            var preProcessedSource = new List<Tuple<int, string>>();

            foreach (var line in source)
            {
                // Strip comments 
                string instruction = RemoveComments(line.Item2, "//");

                // Replace tabs with spaces
                instruction = instruction.Replace('\t', ' ');

                // Remove empty lines
                if (!String.IsNullOrEmpty(instruction))
                {
                    preProcessedSource.Add(new Tuple<int, string>(line.Item1, instruction));
                }
            }

            // Convert labels to addresses
            preProcessedSource = GatherLabels(preProcessedSource);

            return preProcessedSource;
        }


        // Converts all labels to addresses
        private List<Tuple<int, string>> GatherLabels(List<Tuple<int, string>> source)
        {
            var preProcessedSource = new List<Tuple<int, string>>();
            for(int i = 0; i < source.Count; i++)
            {
                var line = source[i];

                // If the line is a label then add the label to our dictionary and remove it from the source
                if(line.Item2.Contains(' ') == false && line.Item2.Contains('\t') == false && line.Item2.EndsWith(':'))
                {
                    Labels.Add(line.Item2, i);
                }
                else 
                {
                    preProcessedSource.Add(new Tuple<int, string>(line.Item1, line.Item2));
                }
            }

            // Do a second pass and replace any label references with the address to jump to
            var preProcessedSourceWithLabelJumps = new List<Tuple<int, string>>();
            foreach (var line in preProcessedSource)
            {
                // Very inefficient! 
                // If we have a label on the line then go through all our stored labels, when you find a match replace it with the program address to jump to
                // TODO: Will break if multiple labels on one line
                if(line.Item2.EndsWith(':'))
                {
                    foreach(var label in Labels)
                    {
                        if (line.Item2.Contains(label.Key))
                        {
                            preProcessedSourceWithLabelJumps.Add(new Tuple<int, string>(line.Item1, line.Item2.Replace(label.Key, label.Value.ToString())));
                            break;
                        }
                    }
                }
                else 
                {
                    preProcessedSourceWithLabelJumps.Add(line);
                }
            }

            return preProcessedSourceWithLabelJumps;
        }


        private string RemoveComments(string str, string delimiter)
        {
            // Regular expression to find a character (delimiter) and replace it and everything following it with an empty string.
            // Trim() will remove all beginning and ending white space.
            return Regex.Replace(str, delimiter + ".+", string.Empty).Trim();
        }



    }
}
