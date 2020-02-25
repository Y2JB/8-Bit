using System;

using Xunit;
using Moq;

using EightBitSystem;
using Simulator;


namespace Simulator.Test
{
    public class AluTest
    {

        [Theory]
        [InlineData(2, 1, true)]
        [InlineData(1, 1, false)]
        [InlineData(0, 255, false)]
        [InlineData(255, 254, true)]
        public void Arithmetic(byte aValue, byte bValue, bool sub)
        {
            var bus = new Mock<IBus>();
            var controlUnit = new Mock<IControlUnit>();
            var aReg = new Mock<IRegister>();
            var bReg = new Mock<IRegister>();

            aReg.SetupGet(x => x.Value).Returns(aValue);
            bReg.SetupGet(x => x.Value).Returns(bValue);

            ControlLine subtractLine = new ControlLine(ControlLineId.SUBTRACT);
            ControlLine busOutputLine = new ControlLine(ControlLineId.SUM_OUT);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.SUBTRACT)).Returns(subtractLine);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.SUM_OUT)).Returns(busOutputLine);

            // Are we adding or subtracting?
            subtractLine.State = sub;

            var alu = new Alu(bus.Object, controlUnit.Object, aReg.Object, bReg.Object);

            if (sub)
            {
                Assert.Equal(aValue - bValue, alu.Value);
            }
            else
            {
                Assert.Equal(aValue + bValue, alu.Value);
            }

            Assert.False(alu.Zero);
            Assert.False(alu.Carry);
        }


        [Theory]
        [InlineData(255, 1, false)]
        [InlineData(-10, 255, true)]
        public void ValueShouldCarry(int aValue, int bValue, bool sub)
        {
            var bus = new Mock<IBus>();
            var controlUnit = new Mock<IControlUnit>();
            var aReg = new Mock<IRegister>();
            var bReg = new Mock<IRegister>();

            aReg.SetupGet(x => x.Value).Returns((byte) aValue);
            bReg.SetupGet(x => x.Value).Returns((byte) bValue);

            ControlLine subtractLine = new ControlLine(ControlLineId.SUBTRACT);
            ControlLine busOutputLine = new ControlLine(ControlLineId.SUM_OUT);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.SUBTRACT)).Returns(subtractLine);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.SUM_OUT)).Returns(busOutputLine);

            // Are we adding or subtracting?
            subtractLine.State = sub;

            var alu = new Alu(bus.Object, controlUnit.Object, aReg.Object, bReg.Object);
            
            // Carry will return the carried value not the intended value 
            if(sub)
            {
                Assert.NotEqual(aValue - bValue, alu.Value);
            }
            else
            {
                Assert.NotEqual(aValue + bValue, alu.Value);
            }

            
            Assert.True(alu.Carry);
        }


        [Theory]
        [InlineData(255, 255, true)]
        [InlineData(128, 128, true)]
        [InlineData(1, 1, true)]
        [InlineData(0, 0, true)]
        [InlineData(0, 0, false)]
        public void ValueShouldZero(byte aValue, byte bValue, bool sub)
        {
            var bus = new Mock<IBus>();
            var controlUnit = new Mock<IControlUnit>();
            var aReg = new Mock<IRegister>();
            var bReg = new Mock<IRegister>();

            aReg.SetupGet(x => x.Value).Returns(aValue);
            bReg.SetupGet(x => x.Value).Returns(bValue);

            ControlLine subtractLine = new ControlLine(ControlLineId.SUBTRACT);
            ControlLine busOutputLine = new ControlLine(ControlLineId.SUM_OUT);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.SUBTRACT)).Returns(subtractLine);
            controlUnit.Setup(x => x.GetControlLine(ControlLineId.SUM_OUT)).Returns(busOutputLine);

            // Are we adding or subtracting?
            subtractLine.State = sub;

            var alu = new Alu(bus.Object, controlUnit.Object, aReg.Object, bReg.Object);

            Assert.Equal(0, alu.Value);
            Assert.True(alu.Zero);
        }
    }

}
