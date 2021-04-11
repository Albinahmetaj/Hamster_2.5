using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Objects
{

    public class Hamster
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hamsterID { get; set; }
        public string Namn { get; set; }
        public int Ålder { get; set; }

        public string Kön { get; set; }
        public string ÄgarNamn { get; set; }


        //public enum GetKön
        //{
        //    M,
        //    K
        //}
        //public GetKön Kön { get; set; }
        public string IdentityNum { get; set; }
        public DateTime CheckIn { get; set; }
        public int MotionsNivå { get; set; }
        public virtual HamsterKö HamsterKö { get; set; }
        public virtual Bur Bur { get; set; }
        public virtual Aktivitet Aktivitet { get; set; }
        public virtual Hemfärd Hemfärd { get; set; }
        public virtual ResterandeHamster ResterandeHamster { get; set; }



        public Hamster(/*string name, string ägarnamn, int ålder, string kön, */string identityNum, int motionsNivå)
        {

            GenereraNyHamster();
            //Name = name;
            //ÄgarNamn = ägarnamn;
            //Ålder = ålder;
            //Kön = kön;
            IdentityNum = identityNum;
            MotionsNivå = motionsNivå;
            CheckIn = CalculateCheckIn(identityNum);
        }

        public Hamster()
        {

        }

        public void GenereraNyHamster()
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

            List<int> ålder = new List<int>()
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30
            };

            List<string> kön = new List<string>();
            kön.Add("M");
            kön.Add("K");

            List<string> ägare = new List<string>()
            {
                "Janne Karlsson","Julia Andersson","Mikaela Johansson", "Carl Hamilton", "Lotta Lorandsson", "Lisa Nilsson", "Jan Hallgren", "Sauron Darksidesson", "Elrond Alvensson", "Emil Helldal", "John Benson", "Viktor Isaksson",
                                    "Tom Nordh", "Elisabeth Eliasson", "Belinda Larsson", "Vlera Karlsson", "Berte Qvarn", "Johanna Johansson", "Martina Löfskog", "Jennifer Lawrence", "Megan Fox", "Zlatan Ibrahimovic", "Anna Al",
                                    "Mehmet Aljuro", "Carita Gran", "Mia Eriksson","Anna Turesson","Lennart Berg","Bo Bergman", "Jolie Andersson"

            };



            var rnd = new Random();
            int r = new Random().Next(0, hamstrar.Count);
            this.Namn = "";
            this.Ålder = ålder[rnd.Next(0, ålder.Count)];
            this.Kön = kön[rnd.Next(0, kön.Count)].ToUpper();
            this.ÄgarNamn = "";

            Namn += hamstrar[r].ToUpper();

            ÄgarNamn += ägare[r].ToUpper();
            hamstrar.RemoveAt(r);
            ägare.RemoveAt(r);

        }

        public override string ToString()
        {
            return $"{this.Namn}, ägarnamn: {this.ÄgarNamn}, ålder {this.Ålder}, kön {this.Kön}%";
        }

        /// <summary>
        /// Calculates age from the randomized SSN
        /// </summary>
        /// <param name="identityNum">the SSN to convert to a birthdate</param>
        /// <returns>Time of birth</returns>
        public DateTime CalculateCheckIn(string identityNum)
        {
            string checkInDate = identityNum.Remove(8, 5);
            DateTime dateValue = DateTime.ParseExact(checkInDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            return dateValue;
        }

    }
}
