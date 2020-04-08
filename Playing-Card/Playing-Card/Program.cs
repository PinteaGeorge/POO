using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playing_Card
{
    class Program
    {
        static void Main(string[] args)
        {
            PlayingCard p1 = new PlayingCard(4, 13);
            Console.WriteLine(p1+  p1.SuitToString() +", "+p1.RankToString());
            Console.ReadKey();
        }
       
    }
}
