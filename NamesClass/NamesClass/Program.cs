using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamesClass
{
    class Program
    {
        static void Main(string[] args)
        {
            Names names = new Names(10);

            Console.WriteLine(names[2]);
            Console.WriteLine(names[-3]);
            Console.WriteLine(names[12]);
            names[2] = "New Name 2";
            Console.WriteLine(names[2]);

            Console.WriteLine(names["New Name 2"]);
            Console.WriteLine(names["Some name"]);
            names["New Name 2"] = "Newest name 2";
            Console.WriteLine(names["Newest name 2"]);

            Console.ReadLine();
        }
    }
}
