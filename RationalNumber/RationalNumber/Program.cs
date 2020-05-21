using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RationalNumber
{
    class Program
    {
        static void Main(string[] args)
        {
            Rational r1 = new Rational(4, -2);
            Rational r2 = new Rational(3, 5);
            Rational r3 = new Rational(6, 4);
            Console.WriteLine((r1 + r2 * r3 - (r2 ^ 3) + r1).SimplestForm());
            Console.ReadLine();
        }
    }
}
