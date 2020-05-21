using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamesClass
{
    public class Names
    {
        private readonly List<string> names;
        public string this[int index] { get => GetNameByIndex(index); set => SetNameByIndex(index, value); }
        public string this[string name] { get => GetNameByName(name); set => SetNameByName(name, value); }

        public Names(int count)
        {
            names = new List<string>();
            for (int i = 0; i < count; i++)
                names.Add($"RandomName{i}");
        }
        private string GetNameByIndex(int index)
        {
            try
            {
                string name = names[index];
                return name;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Index was out of range.");
                return null;
            }
            catch(Exception)
            {
                Console.WriteLine("Something went wrong, the name could not be received.");
                return null;
            }
        }

        private void SetNameByIndex(int index, string value)
        {
            try
            {
                names[index] = value;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Index was out of range.");
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong, the name could not be assigned.");
            }
        }

        private string GetNameByName(string findName)
        {
            try
            {
                string name = names.First(n => n == findName);
                return name;
            }
            catch (Exception)
            {
                Console.WriteLine($"Name {findName} was not found.");
                return null;
            }
        }

        private void SetNameByName(string name, string value)
        {
            try
            {
                names[name.IndexOf(name)] = value;
            }
            catch (Exception)
            {
                Console.WriteLine($"Name {value} was not found.");
            }
        }
    }
}
