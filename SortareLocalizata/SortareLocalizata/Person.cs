using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortareLocalizata
{
    public class Person
    {
        public string LastName;
        public string FirstName;
        public string SecondName;
        public Person()
        {
            LastName = null;
            FirstName = null;
            SecondName = null;
        }
        public void CorrectFormat()
        {
            if (LastName != null)
            {
                LastName = LastName.ToLower();
                string name = (char)(LastName[0] - 32) + LastName.Substring(1);
                LastName = name;
            }
            if (FirstName != null)
            {
                FirstName = FirstName.ToLower();
                string name = (char)(FirstName[0] - 32) + FirstName.Substring(1);
                FirstName = name;
            }
            if (SecondName != null)
            {
                SecondName = SecondName.ToLower();
                string name = (char)(SecondName[0] - 32) + SecondName.Substring(1);
                SecondName = name;
            }
        }
    }
}
