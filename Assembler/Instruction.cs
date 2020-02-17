using System;

using EightBitSystem;

namespace asm
{
    public class Instruction
    {
        public OpCode _OpCode { get; set; }


        public string SourceAsm { get; private set; }
        public int LineNumber { get; private set; }

        public byte Value { get { return (byte) _OpCode; } }


        public Instruction(int lineNumber, string sourceAsm)
        {
            LineNumber = lineNumber;
            SourceAsm = sourceAsm;

            Parse();
        }


        public void Parse()
        {
            OpCode opCode;
            if (!Enum.TryParse(SourceAsm, out opCode))
            {
                throw new Exception(String.Format("ERROR: Line {0} : Unknown OpCode", LineNumber));
            }

            _OpCode = opCode;               
        }


    }
}
