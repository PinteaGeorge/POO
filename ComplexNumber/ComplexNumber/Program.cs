using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexNumber
{
    class Program
    {
        static void Main(string[] args)
        {
            Complex c1 = new Complex(3, 2);
            Complex c2 = new Complex(5, 2);
            Complex c3 = new Complex(5, 11);
            Console.WriteLine(c1 + c2 - c3 * (c1 ^ 3) - c2);
            Console.WriteLine();

            ComplexD cd1 = new ComplexD(2, 5);
            Console.WriteLine(cd1.PowerInTrigonometricForm(5));
            Console.WriteLine();

            ComplexD cd2 = new ComplexD(5, 8);
            ComplexD cd3 = new ComplexD(-4, 2);
            ComplexD cd4 = new ComplexD(3, 7);
            Console.WriteLine(cd1.DistToComplexArray(cd2, cd3, cd4));
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
