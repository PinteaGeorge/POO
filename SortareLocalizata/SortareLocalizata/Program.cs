using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortareLocalizata
{
    class Program
    {
        public static bool CheckDivider(char c, char[] dividers)
        {
            foreach (char divider in dividers)
                if (c == divider) 
                    return true;
            return false;
        }
        static void Main(string[] args)
        {
            List<Person> people = new List<Person>();
            char[] dividers = { ' ', ',', '.', ';', '/', '\\', '-', '_' };
            string[] lines = File.ReadAllLines("../../input.txt");
            foreach (string line in lines)
            {
                Person p = new Person();
                string name = "";
                bool onWord = false;
                for (int i = 0; i < line.Length; i++) 
                {
                    char c = line[i];
                    if (onWord)
                    {
                        if (CheckDivider(c, dividers) || i == line.Length - 1)
                        {
                            if (i == line.Length - 1)
                                name += c;
                            if (p.LastName == null)
                                p.LastName = name;
                            else if (p.FirstName == null)
                                p.FirstName = name;
                            else if (p.SecondName == null)
                                p.SecondName = name;
                            name = "";
                            onWord = false;
                        }
                        else
                        {
                            name += c;
                        }
                    }
                    else
                    {
                        if (!CheckDivider(c, dividers))
                        {
                            onWord = true;
                            name += c;
                        }
                    }
                }
                p.CorrectFormat();
                people.Add(p);
            }
            people = people.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ThenBy(p => p.SecondName).ToList();
            string output = "";
            foreach (Person person in people)
            {
                output += person.LastName + " " + person.FirstName + " " + person.SecondName + "\n";
            }
            File.WriteAllText("../../output.txt", output);
            Console.ReadLine();
        }
    }
}
