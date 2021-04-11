
using Backend;
using Backend.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hamster_2._5AdvDotNET
{
    // TEST
    //public static class HamsterExtensions
    //{
    //    //public static IEnumerable<Hamster> ToHamster(this IEnumerable<string> source)
    //    //{
    //    //    foreach (var line in source)
    //    //    {
    //    //        var columns = line.Split(',');
    //    //        yield return new Hamster
    //    //        {
    //    //            Namn = columns[0],
    //    //            Ålder = int.Parse(columns[1]),
    //    //            Kön = columns[2],
    //    //            ÄgarNamn = columns[3]
    //    //        };
    //    //    }
    //    //}
    //}
    class Program
    {
        public static Object locker = new Object();
        public static HamsterDagis dagis = new HamsterDagis();
        static void Main(string[] args)
        {
            //Console meny logic för de olika funktionerna
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<HamsterDb>());
            bool menu = true;
            while (menu)
            {
                Console.Clear();
                Console.WriteLine("[1] Starta simulation och lägg till hamstrar ");
                Console.WriteLine("[2] Se historik på hamstrarna ");
                Console.WriteLine("[3] Exit");
                ConsoleKey valdTangent = Console.ReadKey().Key;
                switch (valdTangent)
                {
                    case ConsoleKey.D1:
                        //kopplar metoden log som skall köras när eventet HamsterMovedEvent inträffar
                        dagis.HamsterMovedEvent += Logger.Log;
                        StartThreads();
                        break;

                    case ConsoleKey.D2:
                        Logger.ReadLog();
                        break;

                    case ConsoleKey.D3:
                    default:
                        menu = false;
                        break;


                }
                Console.ReadLine();
            }


        }


        //public static List<Hamster> ProcessHamstrar(string path)
        //{
        //    var query = File.ReadAllLines(path)
        //        .Where(l => l.Length > 1)
        //        .ToHamster();
        //    return query.ToList();
        //}

        //En trådstartar metod som kör igång alla trådar och initieras i main
        public static void StartThreads()
        {
            DateTime dateAndTime = DateTime.Now;
            DateTime date = dateAndTime.Date;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Thread thread1 = new Thread(dagis.AddHamster);
            Thread thread2 = new Thread(MoveHamsterThread);
            Thread thread3 = new Thread(UpdateExerciseThread);
            Thread thread4 = new Thread(MoveHomeOrExerciseThread);
            thread1.Start();
            thread1.Join();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            thread4.Join();
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"Checkar ut hamstrar för idag med datum och tid {date.ToString("dd/MM/yyyy")} 17:00:00");
            Console.Write("Tid för simulering {0} sekunder", stopwatch.ElapsedMilliseconds / 1000);
        }
       
        // Nedanför låser jag två metoder, då har man möjlighet att köra simuleringen utan thread.sleep() i högsta hastighet och den blir fortfarande stabil
        //Om inte så får man ett Exception just för att två av trådarna ändrar på samma entity samtidigt.
        //programmet fungerar utan locks här just för att dem hinner nästan alltid bli klara med databasuppdateringar innan en annan tråd går in och ändrar samma entity.
        public static void MoveHamsterThread()
        {
            while (dagis.HamstersHomecoming < dagis.AmountToSimulate)
            {
                lock (locker)
                {
                    dagis.MoveHamster();
                }
                Thread.Sleep(5000);
            }
        }
        public static void UpdateExerciseThread()
        {
            while (dagis.HamstersHomecoming < dagis.AmountToSimulate)
            {
                dagis.UpdateExerciseLevel();
                Thread.Sleep(3000);
            }
        }
        public static void MoveHomeOrExerciseThread()
        {
            while (dagis.HamstersHomecoming < dagis.AmountToSimulate)
            {
                lock (locker)
                {
                    dagis.LogicForHomeOrStillExercising();
                }
                Thread.Sleep(5000);
            }
        }
    }
   
}

