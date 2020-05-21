using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemaLR
{
    class Program
    {
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            List<int> indexElementsLR = new List<int>();
            int n;
            int maxNr = 1000000;
            do
            {
                Console.Write("Array length (3 - 1 000 000)= "); n = int.Parse(Console.ReadLine());
                if (n < 3) 
                    Console.WriteLine("Enter a bigger value");
                if (n > 1000000) 
                    Console.WriteLine("Enter a smaller value");
            } while (n < 3 || n > 1000000);
            int[] v = new int[n];
            for (int i = 0; i < n; i++)
            {
                v[i] = rnd.Next(maxNr);
                Console.Write(v[i] + " ");
            }
            Console.WriteLine();

            int leftMax = v[0];
            for (int i = 1; i < n - 1; i++)
            {
                if (leftMax <= v[i])
                {
                    bool isLR = true;
                    for (int j = i + 1; j < n; j++)
                    {
                        if (v[j] < v[i])
                        {
                            isLR = false;
                            break;
                        }
                    }
                    if (isLR)
                        indexElementsLR.Add(i);
                }
                if (v[i] > leftMax) 
                    leftMax = v[i];
            }

            if (indexElementsLR.Count() == 0)
                Console.WriteLine("There are no LR elements");
            else
            {
                Console.WriteLine("LR elements:");
                foreach (int index in indexElementsLR)
                    Console.WriteLine($"Number {v[index]} on index {index + 1}");
            }
            Console.ReadLine();
        }
    }
}
