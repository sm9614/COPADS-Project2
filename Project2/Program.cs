using System.Security.Cryptography;
using System.Diagnostics;
using System.Numerics;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3) 
            {
                Console.WriteLine("Error: requires exactly 3 arguments: <bits> <option> <count> ");
                Console.WriteLine("bits - the number of bits of the number to be generated, " +
                "this must be a multiple of 8, and at least 32 bits.");
                Console.WriteLine("option - 'odd' or 'prime' (the type of numbers to be generated)");
                Console.WriteLine("count - the count of numbers to generate, defaults to 1");
                return;
            }

            if (!int.TryParse(args[0], out int bits) || bits % 8 != 0 || bits < 32) 
            {
                Console.WriteLine("Error: Bits must be a number greater 32 and a multiple of 8");
                return;
            }
            string option = args[1].ToLower();

            if (option != "odd" && option != "prime")
            {
                Console.WriteLine("option - 'odd' or 'prime' (the type of numbers to be generated)");
                return;

            }
            
            if (!int.TryParse(args[2], out int count)) 
            {
                Console.WriteLine("Error: Count must be a number");
                return;
            }

            Console.WriteLine("BitLength: " + bits + " bits");

        }

         public void generatingFactors(){}

         public void checkingPrime(){}
    }
}