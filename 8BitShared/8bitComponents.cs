using System;

namespace EightBitSystem
{
    // We will use 5 bits for the op code, giving us a max of 32
    public enum OpCode
    {
        NOP = 0x00,         // Do nothing 
        MOV = 0x01,         // MOV a value into a Register. MOV A 0xFF
        LDR = 0x02,         // Load a register with a value from RAM. LDR A 0xA2
        LDX = 0x03,         // Load a register with a value from ROM. LDX A 7
        STR = 0x04,         // Store register value in memory. STR B 0xF0
        ADD = 0x05,         // Add B to A. ADD
        SUB = 0x06,         // Sub B from A. SUB
        CMP = 0x07,         // Compares the value of the A register with a location in memory, sets the Zero flag if they are the same. Does not disturb any registers
        JMP = 0x08,         // Unconditionally jumps to address. JMP 0xF0            
        JZ  = 0x09,          // Jump if zero flag is set. JZ 0xF0
        JE  = 0x09,          // Maps to the same microcode as JZ but is symantically different (used with CMP). CMP B 0x11 JE foo:
        JNZ = 0x0A,         // Jump if zero flag is not set. JNZ 0x0F
        JNE = 0x0A,         // Maps to the same microcode as JNZ but is symantically different (used with CMP)
        JC  = 0x0B,          // Jump if the carry bit is set. JC 0x01
//      CALL        = 0x0C,         // Pushes the return address into the ret register then jumps to the label. CALL foo:
//      RET         = 0x0D,         // Used to end a function. Loads the PC with the ret register contents. 
        OUT = 0x1E,         // Sends the contents of the regsiter in the lparam to the output register - OUT A / OUT B
        HLT = 0x1F          // Halts the computer
    }


    public enum GeneralPurposeRegisterId
    {
        A = 1,
        B = 2,
    }

    public enum SystemRegister
    {
        A = 1,
        B,
        MAR,
        OUT,
        IR,
        IR_PARAM,
        FLAGS
    }

    public enum AluFlags
    {
        Zero    = 1 << 0,
        Carry   = 1 << 1
    }


    // A single Microinstruction is turns on one or more of these control lines. There are some caveats to remember, namely that you cannot have two
    // things outputting to the bus at the same time (although two things reading is fine)
    // Each EEPROM can only output 8 bits at one time so we have to use a new EEPROM for each 8 control lines
    public enum ControlLineId
    {
 // EEPROM 1
        HLT             = 1 << 0,           // Halt the computer
        PC_IN           = 1 << 1,           // Program counter in (used to JMP). PC <- Bus
        PC_OUT          = 1 << 2,           // Program Counter -> Bus
        PC_ENABLE       = 1 << 3,           // Program Counter will increment on next clock cycle            
        A_REG_IN        = 1 << 4,           // A <- Bus
        A_REG_OUT       = 1 << 5,           // A -> Bus
        SUM_OUT         = 1 << 6,           // ALU -> Bus (outputs the last ADD, SUB, MUL etc to the bus)                       
        SUBTRACT        = 1 << 7,           // ALU output will be Subtract (instead of ADD)            
 // EEPROM 2
        UPDATE_FLAGS    = 1 << 8,           // Flags register will latch the ALU flags on the next clock pulse
        B_REG_IN        = 1 << 9,           // B <- Bus
        B_REG_OUT       = 1 << 10,          // B -> Bus
        OUT_REG_IN      = 1 << 11,          // OUT <- Bus (new value will appear on LCD display)
        IR_IN           = 1 << 12,          // Instruction Register
        IR_PARAM_IN     = 1 << 13,          // Instruction Register 8 Bit Operand <- Bus
        IR_PARAM_OUT    = 1 << 14,          // Instruction Register 8 Bit Operand -> Bus
        MAR_IN          = 1 << 15,          // Memory Address Register will latch in a new value from the bus on the next clock cycle
// EEPROM 3
        RAM_IN          = 1 << 16,          // RAM will store the value currently on the bus at the address pointed to by the MAR on next clock cycle
        RAM_OUT         = 1 << 17,          // RAM will put the value at address [MAR] onto the bus. RAM[MAR] -> Bus
        ROM_OUT         = 1 << 18,          // ROM will put the value at address [MAR] onto the bus. ROM[MAR] -> Bus
        ROM_BANK_1      = 1 << 19,          // Sets base ROM memory address to 256. Used to read instruction param (IR_PARAM_OUT). Could also be used to read and write memory beyond 256 bytes
    }
}
