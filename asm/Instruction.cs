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
            LDX         = 0x03,         // Load a register with a value from ROM. LDX A 7
            STR         = 0x04,         // Store register value in memory. STR B 0xF0
            ADD         = 0x05,         // Add B to A. ADD
            SUB         = 0x06,         // Sub B from A. SUB
            CMP         = 0x07,         // Compares the value of the A register with a location in memory, sets the Zero flag if they are the same. Does not disturb any registers
            JMP         = 0x08,         // Unconditionally jumps to address. JMP 0xF0            
            JZ          = 0x09,         // Jump if zero flag is set. JZ 0xF0
            JE          = 0x09,         // Maps to the same microcode as JZ but is symantically different (used with CMP). CMP B 0x11 JE foo:
            JNZ         = 0x0A,         // Jump if zero flag is not set. JNZ 0x0F
            JNE          = 0x0A,        // Maps to the same microcode as JNZ but is symantically different (used with CMP)
            JC          = 0x0B,         // Jump if the carry bit is set. JC 0x01
            //CALL        = 0x0C,         // Pushes the return address into the ret register then jumps to the label. CALL foo:
            //RET         = 0x0D,         // Used to end a function. Loads the PC with the ret register contents. 
            OUT         = 0x1E,         // Sends the contents of the regsiter in the lparam to the output register - OUT A / OUT B
            HLT         = 0x1F          // Halts the computer
        }

        public enum InstructionFlags
        {
            Zero        = 1 << 0,
            Carry       = 1 << 1
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
