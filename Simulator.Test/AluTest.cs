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
        [InlineData(255, 1, false)]
        [InlineData(0, 256, true)]
        public void ValueShouldCarry(byte aValue, byte bValue, bool sub)
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

            var alu = new Alu(controlUnit.Object, bus.Object, aReg.Object, bReg.Object);
            
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
        public void ShouldZero(int a, int b, bool sub)
        {
        }
    }

}
