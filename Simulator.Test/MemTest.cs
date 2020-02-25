using System;

using Xunit;
using Moq;

using EightBitSystem;
using Simulator;


namespace Simulator.Test
{
    public class MemTest
    {

        [Fact]
        public void WriteRomByte()
        {
            byte address = 0x04;

            var bus = new Mock<IBus>();
            var controlUnit = new Mock<IControlUnit>();
            var marReg = new Mock<IRegister>();

            marReg.SetupGet(x => x.Value).Returns(address);
 
            ControlLine romBank1Line = new ControlLine(ControlLineId.ROM_BANK_1);
            ControlLine busOutputLine = new ControlLine(ControlLineId.ROM_OUT);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.ROM_BANK_1)).Returns(romBank1Line);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.ROM_OUT)).Returns(busOutputLine);

            var rom = new Rom(bus.Object, controlUnit.Object, marReg.Object);

            // You cannot write to ROM
            Assert.Throws<InvalidOperationException>(() => rom.Write(0xFF)); 
        }


        [Fact]
        public void ReadWriteRamByte()
        {
            byte address = 0x04;
            byte value = 0x77;

            var bus = new Mock<IBus>();
            var controlUnit = new Mock<IControlUnit>();
            var marReg = new Mock<IRegister>();

            marReg.SetupGet(x => x.Value).Returns(address);

            ControlLine busInputLine = new ControlLine(ControlLineId.RAM_IN);
            ControlLine busOutputLine = new ControlLine(ControlLineId.RAM_OUT);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.RAM_IN)).Returns(busInputLine);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.RAM_OUT)).Returns(busOutputLine);

            var ram = new Ram(bus.Object, controlUnit.Object, marReg.Object);

            // Write and then read a byte from RAM, check they are consistent 
            ram.Write(value);
            byte read = ram.Read();

            Assert.Equal(read, value);
        }


        [Fact]
        public void RamReadValueFromBus()
        {
            byte address = 0x04;
            byte expectedValue = 0x55;

            var bus = new Mock<IBus>();
            var controlUnit = new Mock<IControlUnit>();
            var marReg = new Mock<IRegister>();

            bus.SetupGet(x => x.Value).Returns(expectedValue);

            marReg.SetupGet(x => x.Value).Returns(address);

            ControlLine busInputLine = new ControlLine(ControlLineId.RAM_IN);
            ControlLine busOutputLine = new ControlLine(ControlLineId.RAM_OUT);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.RAM_IN)).Returns(busInputLine);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.RAM_OUT)).Returns(busOutputLine);

            var ram = new Ram(bus.Object, controlUnit.Object, marReg.Object);

            // Read from the bus
            busInputLine.State = true;
            busInputLine.onTransition();

            byte read = ram.Read();

            Assert.Equal(expectedValue, read);
        }

    }

}
