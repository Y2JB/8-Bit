using System;


namespace asm
{
    public class Instruction
    {
        // We will use 5 bits for the op code, giving us a max of 32
        public enum OpCode
        {
            NOP         = 0x00,         // Do nothing 
            MOV         = 0x01,         // MOV a value into a Register. MOV A 0xFF
            LDR         = 0x02,         // Load a register with a value from RAM. LDR A 0xA2
            LDX         = 0x03,         // Load a register with a value from ROM. LDX A 0xA2
            STR         = 0x04,         // Store register value in memory. STR B 0xF0
            ADD         = 0x05,         // Add B to A. ADD
            SUB         = 0x06,         // Sub B from A. SUB
            JMP         = 0x07,         // Jumps to address. JMP 0xF0
            JZ          = 0x08,         // Jump if zero flag is set. JZ 0xF0
            JNZ         = 0x09,         // Jump if zero flag is not set. JNZ 0x0F
            OUT         = 0x0A,         // Sends the contents of the regsiter in the lparam to the output register - OUT A / OUT B
            HLT         = 0x1F          // Halts the computer
        }


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
