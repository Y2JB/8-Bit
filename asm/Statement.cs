using System;
using System.Collections.Generic;
using System.Text;

namespace asm
{
    class Statement
    {
        public Instruction instruction { get; private set; }
        public InstructionParameter LeftParam { get; private set; }
        public InstructionParameter RightParam { get; private set; }

        public int LineNumber { get; private set; }    
        public string SourceAsm { get; private set; }

        public int ParameterCount { get { int count = 0; if (LeftParam.Type != InstructionParameter.ParamType.Unused) count++; if (RightParam.Type != InstructionParameter.ParamType.Unused) count++; return count; } }

        public Statement(int lineNumber, string sourceAsm)
        {
            LineNumber = lineNumber;
            SourceAsm = sourceAsm;

            Parse();
        }


        private void Parse()
        {
            string[] components = SourceAsm.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            if (components.Length == 0 || components.Length > 3)
            {
                throw new Exception(String.Format("ERROR: Line {0} : Too many arguments", LineNumber));
            }

            // Parse the op code and 2 params
            instruction = new Instruction(LineNumber, components[0]);


            // If params are passed a null string to parse they will default to 'unused' type and value of zero
            string lParam = (components.Length >= 2) ? components[1] : null;
            string rParam = (components.Length == 3) ? components[2] : null;


            // Bit of a hack for jump instructions
            // JMP has one param but the parm has to be forced into the 8 bit rparam
            if (instruction._OpCode == Instruction.OpCode.JMP ||
                instruction._OpCode == Instruction.OpCode.JZ ||
                instruction._OpCode == Instruction.OpCode.JNZ)
            {
                rParam = lParam;
                lParam = null;
            }

            LeftParam = new InstructionParameter(LineNumber, lParam);
            RightParam = new InstructionParameter(LineNumber, rParam);        

            Validate();
        }


        private void Validate()
        {
            // LParam is limited to using 3 bits (and 5 bits for the op code)
            if(LeftParam.Value > 7)
            {
                throw new Exception(String.Format("ERROR: Line {0} : Left Param exceeds the 3 bit limit (0-7)", LineNumber));
            }

            // Further, we are actually only using 0 & 1 for the A & B registers so anything greater than 1 is invalid
            if (LeftParam.Value > 2)
            {
                throw new Exception(String.Format("ERROR: Line {0} : Unknown register", LineNumber));
            }

            switch (instruction._OpCode)
            {
                case Instruction.OpCode.OUT:
                    if(ParameterCount != 1)
                    {
                        throw new Exception(String.Format("ERROR: Line {0} : Too many parameters on OUT instruction", LineNumber));
                    }
                    break;

                case Instruction.OpCode.MOV:
                    if (ParameterCount != 2)
                    {
                        throw new Exception(String.Format("ERROR: Line {0} : Not enough parameters for MOV instruction", LineNumber));
                    }
                    if(LeftParam.Type != InstructionParameter.ParamType.Reg)
                    {
                        throw new Exception(String.Format("ERROR: Line {0} : MOV must have a Register for Param 1", LineNumber));
                    }
                    if (RightParam.Type != InstructionParameter.ParamType.Int)
                    {
                        throw new Exception(String.Format("ERROR: Line {0} : MOV literal must have a Number for Param 2", LineNumber));
                    }
                    break;
             
                case Instruction.OpCode.JMP:
                case Instruction.OpCode.JZ:
                case Instruction.OpCode.JNZ:
                    if (LeftParam.Type != InstructionParameter.ParamType.Unused || RightParam.Type != InstructionParameter.ParamType.Int)
                    {
                        throw new Exception(String.Format("ERROR: Line {0} : JMP, JZ & JNZ must have one (R) parameter and it must be a valid address", LineNumber));
                    }
                    break;

                case Instruction.OpCode.LDR:
                    if (LeftParam.Type != InstructionParameter.ParamType.Reg || RightParam.Type != InstructionParameter.ParamType.Int)
                    {
                        throw new Exception(String.Format("ERROR: Line {0} : LDR requires REG INT", LineNumber));
                    }
                    break;

                case Instruction.OpCode.STR:
                    if (LeftParam.Type != InstructionParameter.ParamType.Reg || RightParam.Type != InstructionParameter.ParamType.Int)
                    {
                        throw new Exception(String.Format("ERROR: Line {0} : STR requires REG INT", LineNumber));
                    }
                    break;

            }
        }

    }
}
