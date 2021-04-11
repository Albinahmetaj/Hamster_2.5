
using Backend;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hamster_2._5AdvDotNET
{
    //public static class HamsterExtensions
    //{
    //    public static IEnumerable<Patient> ToHamster(this IEnumerable<string> source)
    //    {
    //        foreach (var line in source)
    //        {
    //            var columns = line.Split(',');
    //            yield return new Patient
    //            {
    //                Name = columns[0],
    //                Ålder = int.Parse(columns[1]),
    //                Kön = columns[2],
    //                ÄgarNamn = columns[3]
    //            };
    //        }
    //    }
    //}
    class Program
    {
        public static Object locker = new Object();
        public static HamsterDagis dagis = new HamsterDagis();
        static void Main(string[] args)
        {
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
                        dagis.HamsterMovedEvent += Logger.Log;
                        RunThreads();
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


        //public static List<Patient> ProcessHamstrar(string path)
        //{
        //    var query = File.ReadAllLines(path)
        //        .Where(l => l.Length > 1)
        //        .ToHamster();
        //    return query.ToList();
        //}
        public static void RunThreads()
        {
            DateTime dateTime = new DateTime(2021, 4, 11, 17, 00, 00);
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
            Console.WriteLine("Checkar ut hamstrar för idag med datum och tid {0}", dateTime);
            Console.Write("Tid för simulering {0} sekunder", stopwatch.ElapsedMilliseconds / 1000);
        }
        //NEDAN LÅSER JAG MOVEPATIENTS OCH moveDeadOrHealthyPatients FÖR DÅ KAN MAN KÖRA SIMULERINGEN UTAN THREAD SLEEPS I MAXHASTIGHET OCH DEN BLIR
        //ÄNDÅ KORREKT.
        //Annars får man OptimisticConcurrencyException för att två trådar ändrar på samma entity samtidigt.
        //I thread.sleep settings vi fick i tentabeskrivningen fungerar dock programmet UTAN locks här för dom hinner nästan alltid bli klara
        //med databasupppdateringar innan en annan tråd går in och ändrar i samma enitity.

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

