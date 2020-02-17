using System;

namespace Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var reg = new Register8Bit();
            reg.Value = 15;

            bool bit = reg.GetBit(0);
            bit = reg.GetBit(1);
            bit = reg.GetBit(2);
            bit = reg.GetBit(3);

            Console.WriteLine();
        }
    }
}
