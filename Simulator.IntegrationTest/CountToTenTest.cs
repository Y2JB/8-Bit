using System;
using Xunit;

namespace Simulator.IntegrationTest
{
    public class CountToTenTest
    {
        [Fact]
        public void CountToTen()
        {
            string romFile = "../../../Tests/CountToTen/test.rom";
            string microcodeEeprom0 = "../../../../Sample Microcode/Microcode-Bank0.bin";
            string microcodeEeprom1 = "../../../../Sample Microcode/Microcode-Bank1.bin";
            string microcodeEeprom2 = "../../../../Sample Microcode/Microcode-Bank2.bin";

            EightBitSystem system = new EightBitSystem();
            system.LoadMicrocode(microcodeEeprom0, microcodeEeprom1, microcodeEeprom2);
            system.LoadProgram(romFile);

            system.ControlUnit.OnControlStateUpdated();

            int maxCycles = 1000;
            while(system.Clock.IsHalted == false)
            {
                // Failed to run to completion within a generous limit, probably a problem
                Assert.True(system.Clock.CycleCount <= maxCycles);

                system.Clock.Step();
            }

            Assert.Equal(10, system.Out.Value);
        }
    }
}
