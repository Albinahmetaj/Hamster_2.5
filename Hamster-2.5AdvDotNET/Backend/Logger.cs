using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    public class Logger
    {
        private static int Motionerade = 1;
        private static int HamstersHome = 0;
        private static int HamstersStillExercising = 0;
        public static Object objectLocker = new Object();
        public static void Log(object sender, HamsterEventArgs e)
        {
            //Väldigt sällan kraschar programmet för olika trådar använder denna metod samtidigt. isf får dom vänta med hjälp av denna lock.
            lock (objectLocker)
            {
                using (StreamWriter w = File.AppendText("Filelog.txt"))
                {
                    if (e.Hamster != null)
                    {
                        // Om sanatorium är null och IVA är inte null så befann sig patienten i kön och flyttades till iva.
                        if (e.Hamster.Aktivitet == null && e.Hamster.Bur != null)
                        {
                            Console.WriteLine("Flyttade hamster med namn: {0},ägarnamn: {1}, ålder: {2} och kön {3} till Bur från Kön.", e.Hamster.Namn, e.Hamster.ÄgarNamn, e.Hamster.Ålder, e.Hamster.Kön);
                            w.WriteLine("Flyttade hamster med namn: {0},ägarnamn: {1}, ålder: {2} och kön {3} till Bur från Kön", e.Hamster.Namn, e.Hamster.ÄgarNamn, e.Hamster.Ålder, e.Hamster.Kön);
                            Thread.Sleep(500);
                        }
                        //Om sanatorium inte är null och iva inte är null så flyttade vi patienten från sana till iva.
                        if (e.Hamster.HamsterKö == null && e.Hamster.Aktivitet != null && e.Hamster.Bur != null)
                        {
                            Console.WriteLine("Flyttade hamster med namn: {0}, ägarnamn: {1}, ålder: {2} och kön {3} till Aktivitet från Bur.", e.Hamster.Namn, e.Hamster.ÄgarNamn, e.Hamster.Ålder, e.Hamster.Kön);
                            w.WriteLine("Flyttade hamster med namn: {0}, ägarnamn: {1}, ålder: {2} och kön {3} till Aktivitet från Bur.", e.Hamster.Namn, e.Hamster.ÄgarNamn, e.Hamster.Ålder, e.Hamster.Kön);
                            Thread.Sleep(500);
                        }
                        //Om kö inte är null och sanatorium inte är null flyttade vi från kö till sanatorium.
                        if (e.Hamster.HamsterKö != null && e.Hamster.Aktivitet != null)
                        {
                            Console.WriteLine("Flyttade hamster med namn {0} till Aktivitet från Kön med ägarnamn {1}", e.Hamster.Namn, e.Hamster.ÄgarNamn);
                            w.WriteLine("Flyttade hamster med namn {0} till Aktivitet från Kön med ägarnamn {1}" +
                                "", e.Hamster.Namn, e.Hamster.ÄgarNamn);
                        }
                        //Om afterlife inte är null är han död.
                        if (e.Hamster.Hemfärd != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Hamster med namn {0} gick hem och motionerade {1} gånger", e.Hamster.Namn, e.Hamster.MotionsNivå);
                            w.WriteLine("Hamster med namn {0} gick hem och motionerade {1} gånger", e.Hamster.Namn, e.Hamster.MotionsNivå);
                            HamstersHome++;
                            Console.WriteLine("Totalt hamstrar som gick hem {0}", HamstersHome);

                            Console.ResetColor();
                        }
                        //om Healthy inte är null är patienten frisk.
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
