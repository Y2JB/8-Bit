using System;
using System.Collections.Generic;
using System.Text;

using EightBitSystem;

namespace asm
{
    public class InstructionParameter
    {
        public string SourceAsm { get; private set; }
        public int LineNumber { get; private set; }

        public enum ParamType
        {
            Unused,
            Reg,
            Int,
        }

        public ParamType Type {get; private set; }
        public byte Value { get; private set; }


        public InstructionParameter(int lineNumber, string sourceAsm)
        {
            LineNumber = lineNumber;
            SourceAsm = sourceAsm;
            Type = ParamType.Unused;
            Value = 0;

            if (!String.IsNullOrEmpty(sourceAsm))
            {
                Parse();
            }
        }

        public void Parse()
        {
            int intValue = 0;

            // Param is a register 
            GeneralPurposeRegisterId reg;
            if (Int32.TryParse(SourceAsm, out intValue) == false &&
                Enum.TryParse(SourceAsm, out reg) && 
                Enum.IsDefined(reg.GetType(), reg))
            {             
                Value = (byte)reg;
                Type = ParamType.Reg;
                return;
            }

            // Check if param is a number (hex or dec)            
            if (SourceAsm.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                intValue = Convert.ToInt32(SourceAsm, 16);
                Type = ParamType.Int;
            }
            else if (Int32.TryParse(SourceAsm, out intValue))
            {
                Type = ParamType.Int;
            }

            if(Type == ParamType.Int)
            {
                if (intValue < -128 || intValue > 255)
                {
                    throw new Exception(String.Format("ERROR: Line {0} : Param out of range", LineNumber));
                }

                Value = (byte)intValue;
                return;
            }

            throw new Exception(String.Format("ERROR: Line {0} : Unknown Parameter value - {1}", LineNumber, SourceAsm));
        }


            

            
            

    }
}
