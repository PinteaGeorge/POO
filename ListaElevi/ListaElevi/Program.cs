using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ListaElevi
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
            List<Student> students = new List<Student>();
            char[] dividers = { ' ', ';', '\t'};
            string[] lines = File.ReadAllLines("../../StudentsIn.txt");
            foreach (string line in lines)
            {
                Student student = new Student();
                int state = 0;
                bool onWord = false;
                string word = "";
                int grades = 0;
                float finalGrade = 0f;
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if (onWord)
                    {
                        if (CheckDivider(c, dividers) || i == line.Length - 1)
                        {
                            if (i == line.Length - 1)
                                word += c;
                            if (state == 0)
                            {
                                student.LastName = word;
                                state++;
                            }
                            else if (state == 1)
                            {
                                student.FirstName = word;
                                state++;
                            }
                            else if (state == 2)
                            {
                                grades = int.Parse(word);
                                state++;
                            }
                            else if (state == 3)
                            {
                                finalGrade += float.Parse(word);
                            }
                            word = "";
                            onWord = false;
                        }
                        else
                        {
                            word += c;
                        }
                    }
                    else
                    {
                        if (!CheckDivider(c, dividers))
                        {
                            onWord = true;
                            word += c;
                            if (i == line.Length - 1)
                                finalGrade += float.Parse(word);
                        }
                    }
                }
                student.AverageGrade = finalGrade / grades;
                students.Add(student);
            }
            students = students.OrderByDescending(s => s.AverageGrade).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
            string result = "";
            foreach (Student student in students)
            {
                result += student.LastName + " " + student.FirstName + " " + student.AverageGrade + "\n";
            }
            Console.WriteLine(result);
            File.WriteAllText("../../StudentsOut.txt", result);
            Console.ReadLine();
        }
    }
}

