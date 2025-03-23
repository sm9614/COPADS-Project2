using System.Security.Cryptography;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace Project
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 2 && args.Length < 3)
            {
                Console.WriteLine("Error: requires the following arguments: <bits> <option> <count> ");
                Console.WriteLine("bits - the number of bits of the number to be generated, " +
                "this must be a multiple of 8, and at least 32 bits.");
                Console.WriteLine("option - 'odd' or 'prime' (the type of numbers to be generated)");
                Console.WriteLine("count - the count of numbers to generate, defaults to 1");
                return;
            }

            if (!int.TryParse(args[0], out int bits) || bits % 8 != 0 || bits < 32)
            {
                Console.WriteLine("Error: Bits must be a number 32 or greater and a multiple of 8");
                return;
            }
            string option = args[1].ToLower();

            if (option != "odd" && option != "prime")
            {
                Console.WriteLine("Error: choose odd or prime");
                return;

            }

            int count = 1;
            if (args.Length == 3 && !int.TryParse(args[2], out count))
            {
                Console.WriteLine("Error: Count must be a number");
                return;
            }

            if (option == "odd")
            {
                Console.WriteLine("BitLength: " + bits + " bits");
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < count; i++)
                {
                    BigInteger randomBigInt = Generate.GenerateRandomBigInt(bits);
                    Console.WriteLine(i + 1 + ": " + randomBigInt);
                    Console.WriteLine("Number of factors: " + Generate.GenerateFactors(randomBigInt));
                    Console.WriteLine();
                }
                Console.WriteLine("Time to generate: " + sw.Elapsed.TotalSeconds.ToString());
            }
            else if (option == "prime")
            {
                Console.WriteLine("BitLength: " + bits + " bits");
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < count; i++)
                {
                    BigInteger prime = Generate.GeneratePrime(bits);
                    Console.WriteLine(i + 1 + ": " + prime);
                    Console.WriteLine();
                }
                Console.WriteLine("Time to generate: " + sw.Elapsed.TotalSeconds.ToString());

            }
            else
            {
                Console.WriteLine("option - 'odd' or 'prime' (the type of numbers to be generated)");
            }
        }
    }

    public static class Generate
    {

        public static BigInteger GenerateRandomBigInt(int bits)
        {
            int bytes = bits / 8; // 8 bits is 1 byte
            byte[] randomBytes = new byte[bytes];
            RandomNumberGenerator rand = RandomNumberGenerator.Create();
            lock (rand)
            {
                rand.GetBytes(randomBytes);
            }
            BigInteger randomBigInt = new BigInteger(randomBytes);
            randomBigInt = BigInteger.Abs(randomBigInt) | 1; // forces bigInt to be odd by setting LSB to 1 and positive
            return randomBigInt;
        }

        public static int GenerateFactors(BigInteger bigInt)
        {
            int factors = 0;
            for (int i = 1; i <= BigIntExtension.Sqrt(bigInt); i++)
            {
                if (bigInt % i == 0)
                {
                    if (bigInt / i == i)
                    {
                        factors++;
                    }
                    else
                    {
                        factors += 2;
                    }
                }
            }
            return factors;
        }

        public static BigInteger GeneratePrime(int bits, int k = 10)
        {
            int available = Environment.ProcessorCount;
            while (true)
            {
                BigInteger[] bigInts = new BigInteger[available];
                for (int i = 0; i < available; i++)
                {
                    bigInts[i] = GenerateRandomBigInt(bits);
                }

                BigInteger? prime = null;

                Parallel.For(0, available, (i, loopState) =>
                {
                    if (BigIntExtension.IsProbablyPrime(bigInts[i]))
                    {
                        prime = bigInts[i];
                        loopState.Break();
                    }
                });

                if (prime.HasValue)
                {
                    return prime.Value;
                }
            }
        }
    }


    public static class BigIntExtension
    {

        public static BigInteger Sqrt(BigInteger bigInt)
        {
            BigInteger low = 0;
            BigInteger high = bigInt;
            BigInteger mid;

            // BInary search is used to find a number from 0 to n such that the number^2 is the given bigint 
            while (low <= high)
            {
                mid = (low + high) / 2;
                BigInteger midSquared = mid * mid;
                if (midSquared == bigInt)
                {
                    return mid; // since mid^2 is equal the number the mid is the square root
                }
                else if (midSquared < bigInt)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return high;
        }

        public static Boolean IsProbablyPrime(this BigInteger value, int k = 10)
        {
            int s = 0;
            BigInteger d = value - 1;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }
            using var rand = RandomNumberGenerator.Create();
            byte[] bytes = new byte[value.ToByteArray().Length];

            for (int i = 0; i < k; i++)
            {
                BigInteger a;
                BigInteger range = value - 3; // the numbers between the range 2 and n-2 is upper bound n-2 - lower bound 2 + 1
                while (true)
                {
                    rand.GetBytes(bytes);
                    a = new BigInteger(bytes);
                    a = BigInteger.Abs(a);

                    if (a >= 2 && a <= (value - 2))
                    {
                        break;
                    }
                }

                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1) continue;
                for (int j = 1; j < s; j++)
                {
                    BigInteger y = BigInteger.ModPow(x, 2, value);

                    if (y == 1 && x != 1 && x != value - 1)
                    {
                        return false;
                    }
                    x = y;
                }

                if (x != 1) // since x was set equal to y
                {
                    return false;
                }
            }

            return true;
        }
    }
}