using System;
using System.IO;

using asm;


namespace MicrocodeGen
{
    public class MicroInstructions
    {
        // A single Microinstruction is turns on one or more of these control lines. There are some caveats to remember, namely that you cannot have two
        // things outputting to the bus at the same time (although two things reading is fine)
        // Each EEPROM can only output 8 bits at one time so we have to use a new EEPROM for each 8 control lines
        private enum ControlLine
        {
// EEPROM 1
            HLT             = 1 << 0,           // Halt the computer
            PC_IN           = 1 << 1,           // Program counter in (used to JMP). PC <- Bus
            PC_OUT          = 1 << 2,           // Program Counter -> Bus
            PC_ENABLE       = 1 << 3,           // Program Counter will increment on next clock cycle
            MAR_IN          = 1 << 4,           // Memory Address Register will latch in a new value from the bus on the next clock cycle
            RAM_IN          = 1 << 5,           // RAM will store the value currently on the bus at the address pointed to by the MAR on next clock cycle
            RAM_OUT         = 1 << 6,           // RAM will put the value at address [MAR] onto the bus. RAM[MAR] -> Bus
            ROM_OUT         = 1 << 7,           // ROM will put the value at address [MAR] onto the bus. ROM[MAR] -> Bus
// EEPROM 2
            ROM_BANK_1      = 1 << 8,           // Sets base ROM memory address to 256. Used to read instruction param (IR_PARAM_OUT). Could also be used to read and write memory beyond 256 bytes
            IR_IN           = 1 << 9,           // Instruction Register
            IR_OUT          = 1 << 10,          // Instruction Register -> Bus
            IR_PARAM_IN     = 1 << 11,          // Instruction Register 8 Bit Operand <- Bus
            IR_PARAM_OUT    = 1 << 12,          // Instruction Register 8 Bit Operand ->Bbus
            A_REG_IN        = 1 << 13,          // A <- Bus
            A_REG_OUT       = 1 << 14,          // A -> Bus
            B_REG_IN        = 1 << 15,          // B <- Bus
// EEPROM 3
            B_REG_OUT       = 1 << 16,          // B -> Bus
            OUT_REG_IN      = 1 << 17,          // OUT <- bus (new value will appear on LCD display)
            SUM_OUT         = 1 << 18,          // ALU -> Bus (outputs the last ADD, SUB, MUL etc to the bus)                       
            SUBTRACT        = 1 << 19,          // ALU output will be Subtract (instead of ADD)
        }


        public int MaxStepCount { get; private set; }
        private MemoryStream memStreamEeprom0;
        private MemoryStream memStreamEeprom1;
        private MemoryStream memStreamEeprom2;
        private MemoryStream memStreamEeprom3;

        private int microInstructionCount;
        private readonly int maxFlagsValue = 15;

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

            Console.WriteLine(String.Format("{0} micro instructions written.", microInstructionCount));
        }


        private void GenerateMicrocode()
        {

            // Any instruction that has a register parameter needs to generate it's own set of control words for each variant (A,B...)

            // NOP
            Microcode_NOP(Instruction.OpCode.NOP);

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
            //Microcode_CMP();

            // JMP
            Microcode_JMP();

            // JZ
            //Microcode_JZ();

            // JNZ
            //Microcode_JNZ();

            // JE
            //Microcode_JE();

            // JNE
            //Microcode_JNE();

            // CALL
            //Microcode_CALL();

            // RET
            //Microcode_RET();

            // OUT
            Microcode_OUT();

            // HLT
            Microcode_HLT();
        }


        private void Microcode_NOP(Instruction.OpCode opCode)
        {
            int step = GenerateFetchMicrocode(opCode, null, 0);

            UInt16 address = GenerateMicroInstructionAddress(opCode, null, 0, step++);
            UInt32 controlLines = 0;

            WriteEepromBuffers(address, controlLines);
        }


        private void Microcode_MOV()
        {
            Instruction.OpCode opCode = Instruction.OpCode.MOV;

            foreach (InstructionParameter.Register reg in Enum.GetValues(typeof(InstructionParameter.Register)))
            {
                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt16 address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);

                    UInt32 regIn = MapRegisterToControlLineIn(reg);

                    UInt32 controlLines = (UInt32)(ControlLine.IR_PARAM_OUT) | regIn;

                    WriteEepromBuffers(address, controlLines);
                }
            }               
        }


        private void Microcode_LDR()
        {
            Instruction.OpCode opCode = Instruction.OpCode.LDR;

            foreach (InstructionParameter.Register reg in Enum.GetValues(typeof(InstructionParameter.Register)))
            {
                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt32 controlLines = 0;
                    UInt16 address = 0;

                    controlLines = (UInt32)(ControlLine.IR_PARAM_OUT) | (UInt32)(ControlLine.MAR_IN);
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);

                    UInt32 regIn = MapRegisterToControlLineIn(reg);
                    controlLines = (UInt32)(ControlLine.RAM_OUT) | regIn;
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);
                }
            }
        }


        private void Microcode_LDX()
        {
            Instruction.OpCode opCode = Instruction.OpCode.LDX;

            foreach (InstructionParameter.Register reg in Enum.GetValues(typeof(InstructionParameter.Register)))
            {
                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt32 controlLines = 0;
                    UInt16 address = 0;

                    controlLines = (UInt32)(ControlLine.IR_PARAM_OUT) | (UInt32)(ControlLine.MAR_IN);
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);

                    UInt32 regIn = MapRegisterToControlLineIn(reg);
                    controlLines = (UInt32)(ControlLine.ROM_OUT) | regIn;
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);
                }
            }
        }


        private void Microcode_STR()
        {
            Instruction.OpCode opCode = Instruction.OpCode.STR;

            foreach (InstructionParameter.Register reg in Enum.GetValues(typeof(InstructionParameter.Register)))
            {
                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt32 controlLines = 0;
                    UInt16 address = 0;

                    controlLines = (UInt32)(ControlLine.IR_PARAM_OUT) | (UInt32)(ControlLine.MAR_IN);
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);

                    UInt32 regOut = MapRegisterToControlLineOut(reg);
                    controlLines = (UInt32)(ControlLine.RAM_IN) | regOut;
                    address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);
                    WriteEepromBuffers(address, controlLines);
                }
            }
        }


        private void Microcode_ADD()
        {
            Instruction.OpCode opCode = Instruction.OpCode.ADD;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);

                UInt32 controlLines = (UInt32)(ControlLine.SUM_OUT) | (UInt32)(ControlLine.A_REG_IN);

                WriteEepromBuffers(address, controlLines);
            }           
        }


        private void Microcode_SUB()
        {
            Instruction.OpCode opCode = Instruction.OpCode.SUB;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);

                UInt32 controlLines = (UInt32)(ControlLine.SUM_OUT) | (UInt32)(ControlLine.A_REG_IN) | (UInt32)(ControlLine.SUBTRACT);

                WriteEepromBuffers(address, controlLines);
            }
        }


        private void Microcode_JMP()
        {
            Instruction.OpCode opCode = Instruction.OpCode.JMP;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);

                UInt32 controlLines = (UInt32)(ControlLine.PC_IN) | (UInt32)(ControlLine.IR_PARAM_OUT);

                WriteEepromBuffers(address, controlLines);
            }
        }


        private void Microcode_JZ()
        {
            throw new NotImplementedException();
        }


        private void Microcode_JNZ()
        {
            throw new NotImplementedException();
        }


        private void Microcode_OUT()
        {
            Instruction.OpCode opCode = Instruction.OpCode.OUT;

            foreach (InstructionParameter.Register reg in Enum.GetValues(typeof(InstructionParameter.Register)))
            {
                for (byte flags = 0; flags <= maxFlagsValue; flags++)
                {
                    int step = GenerateFetchMicrocode(opCode, reg, flags);

                    UInt16 address = GenerateMicroInstructionAddress(opCode, reg, flags, step++);

                    UInt32 regOut = MapRegisterToControlLineOut(reg);

                    UInt32 controlLines = (UInt32)(ControlLine.OUT_REG_IN) | regOut;

                    WriteEepromBuffers(address, controlLines);
                }
            }
        }


        private void Microcode_HLT()
        {
            Instruction.OpCode opCode = Instruction.OpCode.HLT;

            for (byte flags = 0; flags <= maxFlagsValue; flags++)
            {
                int step = GenerateFetchMicrocode(opCode, null, flags);

                UInt16 address = GenerateMicroInstructionAddress(opCode, null, flags, step++);

                UInt32 controlLines = (UInt32)(ControlLine.HLT);

                WriteEepromBuffers(address, controlLines);
            }
        }


        private int GenerateFetchMicrocode(Instruction.OpCode opCode, Nullable<InstructionParameter.Register> reg, byte flags)
        {
            // Generate the address for the control word based on the Instruction and the current step
            UInt16 addressStep0 = GenerateMicroInstructionAddress(opCode, reg, flags, 0);
            UInt16 addressStep1 = GenerateMicroInstructionAddress(opCode, reg, flags, 1);
            UInt16 addressStep2 = GenerateMicroInstructionAddress(opCode, reg, flags, 2);

            // The fetch steps are the same for every instruction. Note our code is stored in ROM not RAM
            // These steps fetch the next instruction and put it in the instruction register, then fetch the 8 bit parameter (which will be 0x00 is not used),
            // then advance the PC
            UInt32 fetchStep0 = (UInt32)(ControlLine.PC_OUT) | (UInt32)(ControlLine.MAR_IN);
            UInt32 fetchStep1 = (UInt32)(ControlLine.ROM_OUT) | (UInt32)(ControlLine.IR_IN);
            UInt32 fetchStep2 = (UInt32)(ControlLine.ROM_OUT) | (UInt32)(ControlLine.ROM_BANK_1) | (UInt32)(ControlLine.IR_PARAM_IN) | (UInt32)(ControlLine.PC_ENABLE);

            WriteEepromBuffers(addressStep0, fetchStep0);
            WriteEepromBuffers(addressStep1, fetchStep1);
            WriteEepromBuffers(addressStep2, fetchStep2);

            int nextStep = 3;
            return nextStep;
        }


        // The control lines that will be switched on are contained in the image at an address generated here
        // We currently generate 11 bit addresses (though we have room for 15 bit addr) They are generated out of 'opCode Param microStep', with least significant bit on the right
        // CCCCCPPPFFFFSSS
        private UInt16 GenerateMicroInstructionAddress(Instruction.OpCode opCode, Nullable<InstructionParameter.Register> reg, byte flags, int microStep)
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
        UInt32 MapRegisterToControlLineIn(InstructionParameter.Register reg)
        {
            switch(reg)
            {
                case InstructionParameter.Register.A:
                    return (UInt32) ControlLine.A_REG_IN;
                   

                case InstructionParameter.Register.B:
                    return (UInt32)ControlLine.B_REG_IN;

                default:
                    throw new Exception("Unknown register value");
            }

        }


        // Used to map which control line we turn on to output to a register
        UInt32 MapRegisterToControlLineOut(InstructionParameter.Register reg)
        {
            switch (reg)
            {
                case InstructionParameter.Register.A:
                    return (UInt32)ControlLine.A_REG_OUT;


                case InstructionParameter.Register.B:
                    return (UInt32)ControlLine.B_REG_OUT;

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
