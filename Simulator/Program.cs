using System;

namespace Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            string romFile = "../../../../Sample ASM/test.rom";
            string microcodeEeprom0 = "../../../../Sample Microcode/Microcode-Bank0.bin";
            string microcodeEeprom1 = "../../../../Sample Microcode/Microcode-Bank1.bin";
            string microcodeEeprom2 = "../../../../Sample Microcode/Microcode-Bank2.bin";

            EightBitSystem system = new EightBitSystem();
            system.LoadMicrocode(microcodeEeprom0, microcodeEeprom1, microcodeEeprom2);
            system.LoadProgram(romFile);
            system.PowerOn();
        }
    }
}
