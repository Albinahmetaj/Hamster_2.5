using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    public class RandomLogic
    {
        public static string GenerateName()
        {


            List<string> hamstrar = new List<string>();

            hamstrar.Add("Albin");
            hamstrar.Add("Alice");
            hamstrar.Add("Ylva");
            hamstrar.Add("Björn");
            hamstrar.Add("Danne");
            hamstrar.Add("Carl");
            hamstrar.Add("Elsa");
            hamstrar.Add("Agnes");
            hamstrar.Add("Hugo");
            hamstrar.Add("Kalle");
            hamstrar.Add("Kurt");
            hamstrar.Add("Saga");
            hamstrar.Add("Erik");
            hamstrar.Add("Leah");
            hamstrar.Add("Ludde");
            hamstrar.Add("Thor");
            hamstrar.Add("Stella");
            hamstrar.Add("Molly");
            hamstrar.Add("Isabelle");
            hamstrar.Add("Nora");
            hamstrar.Add("Melinda");
            hamstrar.Add("Lykke");
            hamstrar.Add("Milo");
            hamstrar.Add("Alvin");
            hamstrar.Add("Melissa");
            hamstrar.Add("Olle");
            hamstrar.Add("Arwen");
            hamstrar.Add("Gimli");
            hamstrar.Add("Gandalf");
            hamstrar.Add("Ash");



            //int index = r.Next(0, hamsterNamn.Count - 1);
            //var name = hamsterNamn[index];
            //hamsterNamn.Remove(name);
            int temp = new Random().Next(0, hamstrar.Count);
            string Name = "";
            Name += hamstrar[temp].ToUpper();

            hamstrar.RemoveAt(temp);

            return Name;
        }

        public static string GenerateÄgarNamn()
        {
            List<string> ägare = new List<string>()
                {
                    "Janne Karlsson","Julia Andersson","Mikaela Johansson", "Carl Hamilton", "Lotta Lorandsson", "Lisa Nilsson", "Jan Hallgren", "Sauron Darksidesson", "Elrond Alvensson", "Emil Helldal", "John Benson", "Viktor Isaksson",
                                        "Tom Nordh", "Elisabeth Eliasson", "Belinda Larsson", "Vlera Karlsson", "Berte Qvarn", "Johanna Johansson", "Martina Löfskog", "Jennifer Lawrence", "Megan Fox", "Zlatan Ibrahimovic", "Anna Al",
                                        "Mehmet Aljuro", "Carita Gran", "Mia Eriksson","Anna Turesson","Lennart Berg","Bo Bergman"

                };
            int temp = new Random().Next(0, ägare.Count);
            string Name = "";
            Name += ägare[temp].ToUpper();
            ägare.RemoveAt(temp);
            return Name;

        }
        public static int GenerateÅlder()
        {
            List<int> hamsterÅlder = new List<int>()
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30
            };


            int ålder = new Random().Next(0, hamsterÅlder.Count);
            hamsterÅlder.RemoveAt(ålder);
            return ålder;
        }

        public static string GenerateKön()
        {
            List<string> kön = new List<string>() { "M", "K" };

            int temp = new Random().Next(0, kön.Count);
            string name = "";
            name += kön[temp].ToUpper();
            return name;
        }
        public static string GenerateIdentity()
        {
            StringBuilder sr = new StringBuilder();
            Random r = new Random();
            int year = r.Next(1940, 2020);
            sr.Append(year);
            int month = r.Next(1, 12);
            if (month < 10)
            {
                sr.Append("0");
            }
            sr.Append(month);
            int day = r.Next(1, 28);
            if (day < 10)
            {
                sr.Append("0");
            }
            sr.Append(day);
            sr.Append("-");
            int lastFour = r.Next(1000, 9999);
            sr.Append(lastFour);

            return sr.ToString();
        }
        public static int GenerateExerciseLevel()
        {
            Random r = new Random();
            int level = r.Next(1, 19);
            return level;
        }
    }
}
