using System;

namespace MicrocodeGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var micro = new MicroInstructions();

            micro.GenerateMicrocode();
            micro.Validate();
            micro.WriteRoms();
        }
    }
}
