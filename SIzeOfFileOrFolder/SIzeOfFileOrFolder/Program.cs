using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIzeOfFileOrFolder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Introduce the path:");
            
                string path=Console.ReadLine();
            SizeOfFileOrFolder f1 = new SizeOfFileOrFolder(path);

            f1.TotalSize();
            Console.ReadKey();
        }
    }
}
