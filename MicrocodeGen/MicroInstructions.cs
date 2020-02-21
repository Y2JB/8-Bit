using System;
using System.IO;

using EightBitSystem;


namespace MicrocodeGen
{
    public class MicroInstructions
    {
        public int MaxStepCount { get; private set; }
        private MemoryStream memStreamEeprom0;
        private MemoryStream memStreamEeprom1;
        private MemoryStream memStreamEeprom2;
        private MemoryStream memStreamEeprom3;

        private int microInstructionCount;
        private readonly int maxFlagsValue = 15;

        private int mostSteps = 0;

        public MicroInstructions()
        {
            MaxStepCount = 5;
            memStreamEeprom0 = new MemoryStream(32 * 1024);
            memStreamEeprom1 = new MemoryStream(32 * 1024);
            memStreamEeprom2 = new MemoryStream(32 * 1024);
            memStreamEeprom3 = new MemoryStream(32 * 1024);

            Console.WriteLine("Generating Microcode");
            GenerateMicrocode();
            Console.WriteLine("Validating Microcode");
            Validate();
            WriteRoms();

            Console.WriteLine(String.Format("{0} micro instructions written. Most steps {1}", microInstructionCount, mostSteps+1));
        }


        private void GenerateMicrocode()
        {

            // Any instruction generate a set of control words for every possible input param and cpu flags. It generates a LOT of microcode and
            // can almost certainly be optimized. 

            // NOP
            Microcode_NOP();

            // MOV
            Microcode_MOV();

            // LDR
            Microcode_LDR();
            
            // LDX
            Microcode_LDX();
            
            // STR
            Microcode_STR();

            // ADD
            Microcode_ADD();

            // SUB
            Microcode_SUB();

            // CMP
            Microcode_CMP();

            // JMP
            Microcode_JMP();

            // JZ
            Microcode_JZ();

            // JNZ
            Microcode_JNZ();

            // JC
            Microcode_JC();

            // CALL
            //Microcode_CALL();

            // RET
            //Microcode_RET();

            // OUT
            Microcode_OUT();

            // HLT
            Microcode_HLT();
        }


        private void Microcode_NOP()
        {
            OpCode opCode = OpCode.NOP;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);
                UInt32 controlLines = 0;

                WriteEepromBuffers(address, controlLines);
            }
        }


        private void Microcode_MOV()
        {
            OpCode opCode = OpCode.MOV;

            foreach (GeneralPurposeRegisterId reg in Enum.GetValues(typeof(GeneralPurposeRegisterId)))
            {
                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt16 address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);

                    UInt32 regIn = MapRegisterToControlLineIn(reg);

                    UInt32 controlLines = (UInt32)(ControlLineId.IR_PARAM_OUT) | regIn;

                    WriteEepromBuffers(address, controlLines);
                }
            }               
        }


        private void Microcode_LDR()
        {
            OpCode opCode = OpCode.LDR;

            foreach (GeneralPurposeRegisterId reg in Enum.GetValues(typeof(GeneralPurposeRegisterId)))
            {
                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt32 controlLines = 0;
                    UInt16 address = 0;

                    controlLines = (UInt32)(ControlLineId.IR_PARAM_OUT) | (UInt32)(ControlLineId.MAR_IN);
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);

                    UInt32 regIn = MapRegisterToControlLineIn(reg);
                    controlLines = (UInt32)(ControlLineId.RAM_OUT) | regIn;
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);
                }
            }
        }


        private void Microcode_LDX()
        {
            OpCode opCode = OpCode.LDX;

            foreach (GeneralPurposeRegisterId reg in Enum.GetValues(typeof(GeneralPurposeRegisterId)))
            {
                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt32 controlLines = 0;
                    UInt16 address = 0;

                    controlLines = (UInt32)(ControlLineId.IR_PARAM_OUT) | (UInt32)(ControlLineId.MAR_IN);
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);

                    UInt32 regIn = MapRegisterToControlLineIn(reg);
                    controlLines = (UInt32)(ControlLineId.ROM_OUT) | regIn;
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);
                }
            }
        }


        private void Microcode_STR()
        {
            OpCode opCode = OpCode.STR;

            foreach (GeneralPurposeRegisterId reg in Enum.GetValues(typeof(GeneralPurposeRegisterId)))
            {
                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt32 controlLines = 0;
                    UInt16 address = 0;

                    controlLines = (UInt32)(ControlLineId.IR_PARAM_OUT) | (UInt32)(ControlLineId.MAR_IN);
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);

                    UInt32 regOut = MapRegisterToControlLineOut(reg);

                    controlLines = (UInt32)(ControlLineId.RAM_IN) | regOut;
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);

                    // I worry that when we end RAM_IN, what if the bus output ends fractionally before the memory write signal is stopped?
                    // Would this mean we get bad writes (probably all zero's)? I've added this step as a precaution to keep the bus active but turn off memory in
                    controlLines = regOut;
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);
                }
            }
        }


        private void Microcode_ADD()
        {
            OpCode opCode = OpCode.ADD;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);
                UInt32 controlLines = (UInt32)(ControlLineId.UPDATE_FLAGS);
                WriteEepromBuffers(address, controlLines);

                address = GenerateMicroInstructionAddress(opCode, null, flags, step++);
                controlLines = (UInt32)(ControlLineId.SUM_OUT) | (UInt32)(ControlLineId.A_REG_IN);
                WriteEepromBuffers(address, controlLines);
            }           
        }


        private void Microcode_SUB()
        {
            OpCode opCode = OpCode.SUB;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);
                UInt32 controlLines = (UInt32)(ControlLineId.UPDATE_FLAGS);

                address = GenerateMicroInstructionAddress(opCode, null, flags, step++);
                controlLines = (UInt32)(ControlLineId.SUM_OUT) | (UInt32)(ControlLineId.A_REG_IN) | (UInt32)(ControlLineId.SUBTRACT);
                WriteEepromBuffers(address, controlLines);
            }
        }


        private void Microcode_CMP()
        {
            OpCode opCode = OpCode.CMP;

            // NB: This instruction takes the maximum of 8 uops (micro instructions)!

            foreach (GeneralPurposeRegisterId reg in Enum.GetValues(typeof(GeneralPurposeRegisterId)))
            {
                UInt32 regIn = MapRegisterToControlLineIn(reg);

                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt32 controlLines = 0;
                    UInt16 address = 0;

                    controlLines = (UInt32)(ControlLineId.IR_PARAM_OUT) | (UInt32)(ControlLineId.MAR_IN);
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);

                    // Store B for later, backup in the IR Param (very cheeky)
                    controlLines = (UInt32)(ControlLineId.B_REG_OUT) | (UInt32)(ControlLineId.IR_PARAM_IN);
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);

                    // Fetch the value to be compared
                    controlLines = (UInt32)(ControlLineId.RAM_OUT) | (UInt32)(ControlLineId.B_REG_IN);
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);

                    // Subtract the compare values, and use the zero flag as an equal flag
                    address = GenerateMicroInstructionAddress(opCode, null, flags, step++);
                    controlLines = (UInt32)(ControlLineId.SUBTRACT) | (UInt32)(ControlLineId.UPDATE_FLAGS);
                    WriteEepromBuffers(address, controlLines);

                    // Restore B
                    controlLines = (UInt32)(ControlLineId.IR_PARAM_OUT) | (UInt32)(ControlLineId.B_REG_IN);
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);
                }             
            }
        }


        private void Microcode_JMP()
        {
            OpCode opCode = OpCode.JMP;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);

                UInt32 controlLines = (UInt32)(ControlLineId.PC_IN) | (UInt32)(ControlLineId.IR_PARAM_OUT);

                WriteEepromBuffers(address, controlLines);
            }
        }


        private void Microcode_JZ()
        {
            OpCode opCode = OpCode.JZ;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);

                UInt32 controlLines = 0;

                // Only jump if the right flag is set, ptherwise it's a NOP
                if ((flags | (byte)AluFlags.Zero) == 1)
                {
                    controlLines = (UInt32)(ControlLineId.PC_IN) | (UInt32)(ControlLineId.IR_PARAM_OUT);
                }

                WriteEepromBuffers(address, controlLines);
            }
        }


        private void Microcode_JNZ()
        {
            OpCode opCode = OpCode.JNZ;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);

                UInt32 controlLines = 0;

                // Only jump if the right flag is set, ptherwise it's a NOP
                if ((flags | (byte) AluFlags.Zero) == 0)
                {
                    controlLines = (UInt32)(ControlLineId.PC_IN) | (UInt32)(ControlLineId.IR_PARAM_OUT);
                }

                WriteEepromBuffers(address, controlLines);
            }
        }


        private void Microcode_JC()
        {
            OpCode opCode = OpCode.JC;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);

                UInt32 controlLines = 0;

                // Only jump if the right flag is set, ptherwise it's a NOP
                if ((flags | (byte)AluFlags.Carry) == 1)
                {
                    controlLines = (UInt32)(ControlLineId.PC_IN) | (UInt32)(ControlLineId.IR_PARAM_OUT);
                }

                WriteEepromBuffers(address, controlLines);
            }
        }


        private void Microcode_OUT()
        {
            OpCode opCode = OpCode.OUT;

            foreach (GeneralPurposeRegisterId reg in Enum.GetValues(typeof(GeneralPurposeRegisterId)))
            {
                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt16 address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);

                    UInt32 regOut = MapRegisterToControlLineOut(reg);

                    UInt32 controlLines = (UInt32)(ControlLineId.OUT_REG_IN) | regOut;

                    WriteEepromBuffers(address, controlLines);
                }
            }
        }


        private void Microcode_HLT()
        {
            OpCode opCode = OpCode.HLT;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);

                UInt32 controlLines = (UInt32)(ControlLineId.HLT);

                WriteEepromBuffers(address, controlLines);
            }
        }


        private int GenerateFetchMicrocode(OpCode opCode, Nullable<GeneralPurposeRegisterId> reg, byte flags)
        {
            int step = 0;
            // Generate the address for the control word based on the Instruction and the current step
            UInt16 addressStep0 = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
            UInt16 addressStep1 = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
            UInt16 addressStep2 = GenerateMicroInstructionAddress(opCode, reg, flags, step++);

            // The fetch steps are the same for every instruction. Note our code is stored in ROM not RAM
            // These steps fetch the next instruction and put it in the instruction register, then fetch the 8 bit parameter (which will be 0x00 is not used),
            // then advance the PC
            UInt32 fetchStep0 = (UInt32)(ControlLineId.PC_OUT) | (UInt32)(ControlLineId.MAR_IN);
            UInt32 fetchStep1 = (UInt32)(ControlLineId.ROM_OUT) | (UInt32)(ControlLineId.IR_IN);
            UInt32 fetchStep2 = (UInt32)(ControlLineId.ROM_OUT) | (UInt32)(ControlLineId.ROM_BANK_1) | (UInt32)(ControlLineId.IR_PARAM_IN) | (UInt32)(ControlLineId.PC_ENABLE);

            WriteEepromBuffers(addressStep0, fetchStep0);
            WriteEepromBuffers(addressStep1, fetchStep1);
            WriteEepromBuffers(addressStep2, fetchStep2);

            return step;
        }


        // The control lines that will be switched on are contained in the image at an address generated here
        // We currently generate 11 bit addresses (though we have room for 15 bit addr) They are generated out of 'opCode Param microStep', with least significant bit on the right
        // CCCCCPPPFFFFSSS
        private UInt16 GenerateMicroInstructionAddress(OpCode opCode, Nullable<GeneralPurposeRegisterId> reg, byte flags, int microStep)
        {
            // Flags are 4 bits
            if(flags > 0x0F)
            {
                throw new Exception("Flags cannot exceed 4 bits");
            }

            // Microstep is 3 bits
            if (microStep > 0x07)
            {
                throw new Exception("Microstep cannot exceed 3 bits");
            }

            UInt16 regValue = (UInt16) (reg.HasValue ? reg.Value : 0);

            UInt16 address = (UInt16)(((UInt16)(opCode) << 10) | regValue << 7 | flags << 4 | microStep);

            // Our Eeproms only have 15 address lines so we cannot address more than this
            if (address > 32767)
            {
                throw new Exception("Microstep address exceeds 15 bits");
            }

            // Keep track of whichever instruction takes the most steps
            if(microStep > mostSteps)
            {
                mostSteps = microStep;
            }

            return address;
        }


        // Takes the generated control line data and writes it to the appropriate EEPROM stream
        private void WriteEepromBuffers(UInt16 address, UInt32 controlLineSignals)
        {
            byte b0, b1, b2, b3;
            b0 = (byte) (controlLineSignals & 0x000000FF);
            b1 = (byte) ((controlLineSignals >> 8) & 0x000000FF);
            b2 = (byte) ((controlLineSignals >> 16) & 0x000000FF);
            b3 = (byte) ((controlLineSignals >> 24) & 0x000000FF);

            if (b3 != 0)
            {
                throw new Exception("Err we're writing to eeprom 4 now???");
            }

            memStreamEeprom0.GetBuffer()[address] = b0;
            memStreamEeprom1.GetBuffer()[address] = b1;
            memStreamEeprom2.GetBuffer()[address] = b2;
            memStreamEeprom3.GetBuffer()[address] = b3;

            microInstructionCount++;
        }


        // Used to map which control line we turn on to read into a register
        UInt32 MapRegisterToControlLineIn(GeneralPurposeRegisterId reg)
        {
            switch(reg)
            {
                case GeneralPurposeRegisterId.A:
                    return (UInt32) ControlLineId.A_REG_IN;
                   

                case GeneralPurposeRegisterId.B:
                    return (UInt32)ControlLineId.B_REG_IN;

                default:
                    throw new Exception("Unknown register value");
            }

        }


        // Used to map which control line we turn on to output to a register
        UInt32 MapRegisterToControlLineOut(GeneralPurposeRegisterId reg)
        {
            switch (reg)
            {
                case GeneralPurposeRegisterId.A:
                    return (UInt32)ControlLineId.A_REG_OUT;


                case GeneralPurposeRegisterId.B:
                    return (UInt32)ControlLineId.B_REG_OUT;

                default:
                    throw new Exception("Unknown register value");
            }

        }


        private void Validate()
        {
            // Make sure max of 1 OUT flag is set per micro instruction

            // EEPROM 4 should be all zeroes
        }


        private void WriteRoms()
        {
            Console.Write("Writing Microcode ROMs...");
            using (FileStream f0 = File.Create("Microcode-Bank0.bin"))
            using (FileStream f1 = File.Create("Microcode-Bank1.bin"))
            using (FileStream f2 = File.Create("Microcode-Bank2.bin"))
            using (FileStream f3 = File.Create("Microcode-Bank3.bin"))
            {
                f0.Write(memStreamEeprom0.GetBuffer());
                f0.Close();

                f1.Write(memStreamEeprom1.GetBuffer());
                f1.Close();

                f2.Write(memStreamEeprom2.GetBuffer());
                f2.Close();

                f3.Write(memStreamEeprom3.GetBuffer());
                f3.Close();
            }
            Console.WriteLine("success");
        }
    }
}
