using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixClass
{
    class Program
    {
        public static Random rnd = new Random();
        static void Main(string[] args)
        {
            Matrix m1 = Matrix.RandomMatrix(3, 3);
            Matrix m2 = Matrix.RandomMatrix(3, 3);
            Console.WriteLine(m1);
            Console.WriteLine(m2);
            Console.WriteLine(m1.Add(m2));
            Console.WriteLine(m1.Subtract(m2));
            Console.WriteLine(m1.Multiply(m2));
            Console.WriteLine(m1.Inverse());
            Console.ReadLine();
        }
    }
}
