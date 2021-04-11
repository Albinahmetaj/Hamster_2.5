using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    // En log som visar nuvarande status på dagiset till console skärmen samt skriver till fil
    public class Logger
    {
        private static int Motionerade = 1;
        private static int HamstersHome = 0;
        private static int HamstersStillExercising = 0;
        public static Object objectLocker = new Object();
        public static void Log(object sender, HamsterEventArgs e)
        {
            //Kan hända att det kraschar just för att programmet  använder denna metod samtidigt i de olika  trådarna. Då kan vi ta hjälp utav denna lock och gör så att trådarna får vänta
            lock (objectLocker)
            {
                using (StreamWriter w = File.AppendText("Filelog.txt"))
                {
                    if (e.Hamster != null)
                    {
                        // Ifall aktivitet är null och bur inte är null så befinner sig hamstern/hamstrarna i kön och flyttas till buren
                        if (e.Hamster.Aktivitet == null && e.Hamster.Bur != null)
                        {
                            Console.WriteLine("Flyttade hamster med namn: {0},ägarnamn: {1}, ålder: {2} och kön {3} till Bur från Kö.", e.Hamster.Namn, e.Hamster.ÄgarNamn, e.Hamster.Ålder, e.Hamster.Kön);
                            w.WriteLine("Flyttade hamster med namn: {0},ägarnamn: {1}, ålder: {2} och kön {3} till Bur från Kö", e.Hamster.Namn, e.Hamster.ÄgarNamn, e.Hamster.Ålder, e.Hamster.Kön);
                            Thread.Sleep(500);
                        }
                        // om aktivitet och bur inte är null så flyttar vi hamstern till aktivitet från bur
                        if (e.Hamster.HamsterKö == null && e.Hamster.Aktivitet != null && e.Hamster.Bur != null)
                        {
                            Console.WriteLine("Flyttade hamster med namn: {0}, ägarnamn: {1}, ålder: {2} och kön {3} till Aktivitet från Bur.", e.Hamster.Namn, e.Hamster.ÄgarNamn, e.Hamster.Ålder, e.Hamster.Kön);
                            w.WriteLine("Flyttade hamster med namn: {0}, ägarnamn: {1}, ålder: {2} och kön {3} till Aktivitet från Bur.", e.Hamster.Namn, e.Hamster.ÄgarNamn, e.Hamster.Ålder, e.Hamster.Kön);
                            Thread.Sleep(500);
                        }
                        

                        //if (e.Hamster.HamsterKö != null && e.Hamster.Aktivitet != null)
                        //{
                        //    Console.WriteLine("Flyttade hamster med namn {0} till Aktivitet från Kön med ägarnamn {1}", e.Hamster.Namn, e.Hamster.ÄgarNamn);
                        //    w.WriteLine("Flyttade hamster med namn {0} till Aktivitet från Kön med ägarnamn {1}" +
                        //        "", e.Hamster.Namn, e.Hamster.ÄgarNamn);
                        //}
                       
                        //Om hemfärd inte är null så har hamstern åkt hem
                        if (e.Hamster.Hemfärd != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Hamster med namn {0} åkte hem från aktivitet och motionerade {1} gånger", e.Hamster.Namn, e.Hamster.MotionsNivå);
                            w.WriteLine("Hamster med namn {0} åkte hem från aktivitet och motionerade {1} gånger", e.Hamster.Namn, e.Hamster.MotionsNivå);
                            HamstersHome++;
                            Console.WriteLine("Totalt hamstrar som gick hem {0}", HamstersHome);

                            Console.ResetColor();
                        }
                        //om resterande hamster som är kvar i dagiset inte är null så betyder det att de inte motionerat ännu
                        if (e.Hamster.ResterandeHamster != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Hamstrar med namn {0} har ännu inte motionerat, väntar tills de motionerat minst 1 gång under vistelsen", e.Hamster.Namn);
                            w.WriteLine("Hamstrar med namn {0} har ännu inte motionerat, väntar tills de motionerat minst 1 gång under vistelsen", e.Hamster.Namn);
                            HamstersStillExercising++;
                            Console.WriteLine("Totalt hamstar som väntas motionera {0}", HamstersStillExercising);

                            Console.ResetColor();
                        }
                        if (e.Hamster.ResterandeHamster != null && e.Hamster.Hemfärd == null)
                        {
                            Console.WriteLine("Hamstrar som väntades motionera med namn {0} har nu motionerat {1} gånger", e.Hamster.Namn, Motionerade);
                            w.WriteLine("Hamstrar som väntades motionera med namn {0} har nu motionerat {1} gånger", e.Hamster.Namn, Motionerade);
                            Motionerade++;
                            Thread.Sleep(1500);
                        }
                    }
                }
            }
        }
        //Läser in all information från föregående simulationer
        public static void ReadLog()
        {
            string[] readLines = File.ReadAllLines(@"C:\Users\Albin\source\repos\Hamster_2.5\Hamster-2.5AdvDotNET\Hamster-2.5AdvDotNET\bin\Debug\Filelog.txt");
            Console.WriteLine("Showing contents of the hamsterlog.txt");
            foreach (var item in readLines)
            {
                Console.WriteLine("\t" + item);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        }
    }
}
