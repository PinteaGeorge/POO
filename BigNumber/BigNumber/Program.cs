using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigNumber
{
    class Program
    {
        public static BigNumber Fibonacci(int n)
        {
            BigNumber a = new BigNumber(0);
            BigNumber b = new BigNumber(1);
            int nr = 2;
            if (n <= 1) return a;
            if (n == 2) return b;
            while (nr < n)
            {
                nr++;
                BigNumber c = a + b;
                a = b.Clone();
                b = c.Clone();
            }
            return b;
        }

        public static BigNumber Factorial(int n)
        {
            BigNumber result = new BigNumber(1);
            while (n > 0)
            {
                result *= new BigNumber(n);
                n--;
            }
            return result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("1000th number of fibonacci series: " + Fibonacci(1000));
            Console.WriteLine();
            Console.WriteLine("factorial of 1000: " + Factorial(1000));
            Console.ReadLine();
        }
    }
}
