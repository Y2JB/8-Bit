using System;

using Xunit;
using Moq;

using EightBitSystem;
using Simulator;

namespace Simulator.Test
{
    public class RegisterTest
    {
        [Theory]
        [InlineData(0xFF,   SystemRegister.A, ControlLineId.A_REG_IN)]
        [InlineData(1,      SystemRegister.B, ControlLineId.B_REG_IN)]
        [InlineData(4,      SystemRegister.OUT, ControlLineId.OUT_REG_IN)]
        [InlineData(16,     SystemRegister.MAR, ControlLineId.MAR_IN)]
        [InlineData(64,     SystemRegister.IR, ControlLineId.IR_IN)]
        [InlineData(0x7F,   SystemRegister.IR_PARAM, ControlLineId.IR_PARAM_IN)]        
        public void ReadValueFromBus(byte expectedValue, SystemRegister registerId, ControlLineId lineId)
        {
            var clock = new Mock<IClock>();
            clock.Setup(x => x.AddConnectedComponent(It.IsAny<IClockConnectedComponent>()));

            var bus = new Mock<IBus>();
            bus.SetupGet(x => x.Value).Returns(expectedValue);

            var controlUnit = new Mock<IControlUnit>();

            ControlLine busInputLine = new ControlLine(lineId);
            controlUnit.Setup(x => x.GetControlLine(lineId)).Returns(busInputLine);

            Register reg = new Register(registerId, clock.Object, bus.Object, controlUnit.Object);

            busInputLine.State = true;
            reg.OnRisingEdge();

            // Register has it's input line high so should read the value currently on the bus
            Assert.Equal(expectedValue, reg.Value);
        }


        [Theory]
        [InlineData(0xFF,   SystemRegister.A, ControlLineId.A_REG_IN)]
        [InlineData(1,      SystemRegister.B, ControlLineId.B_REG_IN)]
        [InlineData(4,      SystemRegister.OUT, ControlLineId.OUT_REG_IN)]
        [InlineData(16,     SystemRegister.MAR, ControlLineId.MAR_IN)]
        [InlineData(64,     SystemRegister.IR, ControlLineId.IR_IN)]
        [InlineData(0x7F,   SystemRegister.IR_PARAM, ControlLineId.IR_PARAM_IN)]
        public void NoBusRead(byte expectedValue, SystemRegister registerId, ControlLineId lineId)
        {
            var clock = new Mock<IClock>();
            clock.Setup(x => x.AddConnectedComponent(It.IsAny<IClockConnectedComponent>()));

            var bus = new Mock<IBus>();
            bus.SetupGet(x => x.Value).Returns(expectedValue);

            var controlUnit = new Mock<IControlUnit>();

            ControlLine busInputLine = new ControlLine(lineId);
            controlUnit.Setup(x => x.GetControlLine(lineId)).Returns(busInputLine);

            Register reg = new Register(registerId, clock.Object, bus.Object, controlUnit.Object);

            busInputLine.State = false;
            reg.OnRisingEdge();

            // A register should not read a value when it's controlline is low 
            Assert.Equal(0, reg.Value);
        }
    }
}
