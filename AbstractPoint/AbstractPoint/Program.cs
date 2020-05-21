using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractPoint
{
    class Program
    {
        static void Main(string[] args)
        {
            Point p1 = new Point(PointRepresentation.Rectangular, 1, 1);
            p1.Move(3,3);
            p1.Rotate(3);
            Console.WriteLine(p1);
            Point p2 = new Point(PointRepresentation.Polar, 10, 1.57f);
            p2.Move(1, -2);
            Console.WriteLine(p2);
            Console.ReadLine();
        }
    }
}
