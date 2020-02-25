using System;

using Xunit;
using Moq;

using EightBitSystem;
using Simulator;


namespace Simulator.Test
{
    public class ProgramCounterTest
    {

        [Fact]
        public void PcShouldNotAdvance()
        {
            var clock = new Mock<IClock>();
            clock.Setup(x => x.AddConnectedComponent(It.IsAny<IClockConnectedComponent>()));

            var bus = new Mock<IBus>();
            var controlUnit = new Mock<IControlUnit>();

            ControlLine countEnableLine = new ControlLine(ControlLineId.PC_ENABLE);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_ENABLE)).Returns(countEnableLine);

            ControlLine busOutputLine = new ControlLine(ControlLineId.PC_OUT);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_OUT)).Returns(busOutputLine);

            ControlLine busInputLine = new ControlLine(ControlLineId.PC_IN);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_IN)).Returns(busInputLine);

            var pc = new ProgramCounter(clock.Object, bus.Object, controlUnit.Object);

            byte v1 = pc.Value;
            pc.OnRisingEdge();
            byte v2 = pc.Value;

            Assert.Equal(v1, v2);
        }


        [Fact]
        public void PcShouldAdvance()
        {
            var clock = new Mock<IClock>();
            clock.Setup(x => x.AddConnectedComponent(It.IsAny<IClockConnectedComponent>()));

            var bus = new Mock<IBus>();
            var controlUnit = new Mock<IControlUnit>();

            ControlLine countEnableLine = new ControlLine(ControlLineId.PC_ENABLE);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_ENABLE)).Returns(countEnableLine);

            ControlLine busOutputLine = new ControlLine(ControlLineId.PC_OUT);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_OUT)).Returns(busOutputLine);

            ControlLine busInputLine = new ControlLine(ControlLineId.PC_IN);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_IN)).Returns(busInputLine);

            countEnableLine.State = true;

            var pc = new ProgramCounter(clock.Object, bus.Object, controlUnit.Object);

            byte v1 = pc.Value;
            pc.OnRisingEdge();
            byte v2 = pc.Value;
            pc.OnRisingEdge();
            byte v3 = pc.Value;

            Assert.Equal(0, v1);
            Assert.Equal(1, v2);
            Assert.Equal(2, v3);
        }


        [Fact]
        public void PcReadFromBus()
        {
            byte expectedValue = 0xF0;

            var clock = new Mock<IClock>();
            clock.Setup(x => x.AddConnectedComponent(It.IsAny<IClockConnectedComponent>()));

            var bus = new Mock<IBus>();
            bus.SetupGet(x => x.Value).Returns(expectedValue);

            var controlUnit = new Mock<IControlUnit>();

            ControlLine countEnableLine = new ControlLine(ControlLineId.PC_ENABLE);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_ENABLE)).Returns(countEnableLine);

            ControlLine busOutputLine = new ControlLine(ControlLineId.PC_OUT);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_OUT)).Returns(busOutputLine);

            ControlLine busInputLine = new ControlLine(ControlLineId.PC_IN);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_IN)).Returns(busInputLine);

            busInputLine.State = true;

            var pc = new ProgramCounter(clock.Object, bus.Object, controlUnit.Object);

            pc.OnRisingEdge();
         
            Assert.Equal(expectedValue, pc.Value);
        }


        [Fact]
        public void PcDriveBus()
        {
            byte expectedValue = 0xF0;

            var clock = new Mock<IClock>();
            clock.Setup(x => x.AddConnectedComponent(It.IsAny<IClockConnectedComponent>()));

            var bus = new Mock<IBus>();
            bus.SetupGet(x => x.Value).Returns(expectedValue);            

            var controlUnit = new Mock<IControlUnit>();

            ControlLine countEnableLine = new ControlLine(ControlLineId.PC_ENABLE);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_ENABLE)).Returns(countEnableLine);

            ControlLine busOutputLine = new ControlLine(ControlLineId.PC_OUT);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_OUT)).Returns(busOutputLine);

            ControlLine busInputLine = new ControlLine(ControlLineId.PC_IN);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.PC_IN)).Returns(busInputLine);

            busOutputLine.State = true; 

            var pc = new ProgramCounter(clock.Object, bus.Object, controlUnit.Object);
            bus.SetupSet(m => m.Driver = pc).Verifiable();
            pc.OnRisingEdge();

            bus.Verify();
        }

    }

}
