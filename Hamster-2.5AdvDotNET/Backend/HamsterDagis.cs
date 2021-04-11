using Backend.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    //Den här klassen representerar hamstermetoderna och logiken för hamsterdagiset
    public class HamsterDagis
    {
        // variabel som representerar hur många hamstrar man vill köra igång, ändra denna för fler eller färre.
        public int AmountToSimulate = 30;
        // denna variable är för att styra hela simuleringen, den körs tills hemfärd är större än > AmountToSimulate
        public int HamstersHomecoming = 0;
       
        static HamsterKö CurrentQueue = new HamsterKö();
        static Bur CurrentCage = new Bur();
        static Aktivitet CurrentActivity = new Aktivitet();
        static Hemfärd CurrentHomecoming = new Hemfärd();
        static ResterandeHamster CurrentExercise = new ResterandeHamster();

        // Eventhandler som kallas på när dagiset ska raisa ett/flera event
        public event EventHandler<HamsterEventArgs> HamsterMovedEvent;
      

        internal virtual void OnHamsterMoved(HamsterEventArgs e)
        {
            //raisar eventet
            HamsterMovedEvent?.Invoke(this, e);
        }


        //Metod som lägger till hamstrar genom en for loop, ändra på variablen AmountToSimulate om man vill köra en större eller mindre simulering
        public void AddHamster()
        {
            using (var hamsterDb = new HamsterDb())
            {
                //Datetime för att printa ut dagens datum i consolen
                DateTime dateAndTime = DateTime.Now;
                DateTime date = dateAndTime.Date;
                

                Console.Write("\nLaddar Hamstrar till dagiset");
                Thread.Sleep(1000);
                Console.Write(".");
                Thread.Sleep(1000);
                Console.Write(".");
                Thread.Sleep(1000);
                Console.Write(".");
                Thread.Sleep(1000);
                Console.Write(".");
                Console.WriteLine("");
                Console.WriteLine($"Checkar in hamstrar...Dagens datum och tid är: {date.ToString("dd/MM/yyyy")} 07:00:00");
                Console.WriteLine();
              
                //var Patient = new Hamster(RandomLogic.GenerateIdentity(), RandomLogic.GenerateExerciseLevel());

                //var hamster = Program.ProcessHamstrar("Hamsterlista30.csv");
                //if (!db.Hamster.Any())
                //{
                //    foreach (var item in hamster)
                //    {
                //        db.Hamster.Add(item);
                //        CurrentQueue.HamsterIKö.Add(item);
                //    }
                //}

                for (int i = 0; i < AmountToSimulate; i++)
                {
                    //Blir dubbletter av hamster, vet ej varför (kolla med Paul/Adam för en eventuell lösning)
                    Thread.Sleep(200); 
                    var hamster = new Hamster(RandomLogic.GenerateIdentity(), RandomLogic.GenerateExerciseLevel());
                    hamsterDb.Hamster.Add(hamster);
                    CurrentQueue.HamsterIKö.Add(hamster);

                }

                //Sorterar databasen med hjälp av hamsterID och Motionsnivå, just för att det ska bli lättare att följa under testning. Påverkar ej funktionerna i programmet
                CurrentQueue.HamsterIKö = CurrentQueue.HamsterIKö.OrderByDescending(x => x.hamsterID).ThenBy(x => x.MotionsNivå).ToList(); 
                //Nedanför lägger vi till data i alla avdelningarna i databasen för att göra det möjligt att ladda in dem senare
                hamsterDb.Kö.Add(CurrentQueue);
                hamsterDb.Bur.Add(CurrentCage);
                hamsterDb.Aktivitet.Add(CurrentActivity);
                hamsterDb.Hemfärd.Add(CurrentHomecoming);
                hamsterDb.ResterandeHamster.Add(CurrentExercise);
                
                hamsterDb.SaveChanges();
            }
        }
        //Metod som tar hand om flyttningslogiken mellan de olika avdelningarna/stationerna
        public void MoveHamster()
        {
            using (var hamsterDb = new HamsterDb())
            {
                //Variablernas funktion = laddar in rätt avdelningar från databasen
                var queue = hamsterDb.Kö.Find(CurrentQueue.Id);
                var activity = hamsterDb.Aktivitet.Find(CurrentActivity.Id);
                var cage = hamsterDb.Bur.Find(CurrentCage.Id);


                //Laddar in avdelningarna ifall de har hamstrar i sig
                
                if (queue.HamsterIKö.Count > 0)
                {
                    hamsterDb.Entry(queue).Collection(p => p.HamsterIKö).Load();
                    queue.HamsterIKö = queue.HamsterIKö.OrderByDescending(x => x.hamsterID).ThenBy(x => x.MotionsNivå).ToList();
                }

                if (cage.BurHamster.Count > 0)
                {
                    hamsterDb.Entry(cage).Collection(p => p.BurHamster).Load();
                    cage.BurHamster = cage.BurHamster.OrderByDescending(x => x.hamsterID).ThenBy(x => x.MotionsNivå).ToList();
                }

                if (activity.AktivitetHamster.Count > 0)
                {
                    hamsterDb.Entry(activity).Collection(p => p.AktivitetHamster).Load();
                    activity.AktivitetHamster = activity.AktivitetHamster.OrderByDescending(x => x.hamsterID).ThenBy(x => x.MotionsNivå).ToList();
                }

                hamsterDb.SaveChanges();
                //Fyller på 3 hamstrar i Bur
                while (cage.BurHamster.Count < 3)
                {
                    int result = FindHamster();
                    Hamster hamster;
                    //Om hamster kom från kö
                    if (result == 0)
                    {
                        hamster = queue.HamsterIKö.FirstOrDefault();
                        cage.BurHamster.Add(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //Raisar HamsterMovedEvent att flytt skett
                        queue.HamsterIKö.Remove(hamster);
                    }
                    //Om hamster kom från kö
                    else
                    {
                        hamster = queue.HamsterIKö.FirstOrDefault();
                        cage.BurHamster.Add(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //Raisar HamsterMovedEvent att flytt skett
                        queue.HamsterIKö.Remove(hamster);

                    }
                }
                hamsterDb.SaveChanges();
                //Fyller på 6 hamstrar i aktivitet från bur
                while (activity.AktivitetHamster.Count < 6)
                {
                    var patient = cage.BurHamster.FirstOrDefault();
                    activity.AktivitetHamster.Add(patient);
                    hamsterDb.SaveChanges();

                    OnHamsterMoved(new HamsterEventArgs(patient));
                    cage.BurHamster.Remove(patient);
                }
                hamsterDb.SaveChanges();


            }
        }
        // Metoden hittar den hamstern som väntat längst / ej motionerat eller motionerat minst
        private static int FindHamster()
        {
            using (var hamsterDb = new HamsterDb())
            {
                var queue = hamsterDb.Kö.Find(CurrentQueue.Id);
                var cage = hamsterDb.Bur.Find(CurrentCage.Id);
                var activity = hamsterDb.Aktivitet.Find(CurrentActivity.Id);

                Hamster hamster1 = null;
                Hamster hamster2 = null;
                Hamster hamster3 = null;

                if (queue.HamsterIKö.Count > 0)
                {
                    hamsterDb.Entry(queue).Collection(p => p.HamsterIKö).Load();
                    queue.HamsterIKö = queue.HamsterIKö.OrderByDescending(x => x.hamsterID).ThenBy(x => x.MotionsNivå).ToList();
                    hamster1 = queue.HamsterIKö[0];
                }
                if (cage.BurHamster.Count > 0)
                {
                    hamsterDb.Entry(cage).Collection(p => p.BurHamster).Load();
                    cage.BurHamster = cage.BurHamster.OrderByDescending(x => x.hamsterID).ThenBy(x => x.MotionsNivå).ToList();
                    hamster2 = cage.BurHamster[0];
                }
                
                if (activity.AktivitetHamster.Count > 0)
                {
                    hamsterDb.Entry(activity).Collection(p => p.AktivitetHamster).Load();
                    activity.AktivitetHamster = activity.AktivitetHamster.OrderByDescending(x => x.hamsterID).ThenBy(x => x.MotionsNivå).ToList();
                    hamster3 = activity.AktivitetHamster[0];
                }

                // returnerar 0 för bur och 1 för aktivitet
                // hittar vi hamster från båda avdelningarna så kollar vi den som väntat längst/motionerat minst
                if (hamster2 != null && hamster3 != null)
                {
                    if (hamster2.MotionsNivå.CompareTo(hamster3.MotionsNivå) == 0)
                    {
                        return hamster2.CheckIn < hamster3.CheckIn ? 0 : 1;
                    }
                    return hamster2.MotionsNivå > hamster3.MotionsNivå ? 0 : 1;
                }
                // om aktivitet stationen inte är laddad med hamster så är hamster 3 null och vi gör därför ingen jämföring ovan och returnerar direkt 0 för bur
                else if (hamster2 != null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }
       //Metod uppdaterar hur många gånger hamstrarna motionerat med hjälp utav random funktioner och metoden ExerciseRandomizer
        public void UpdateExerciseLevel()
        {
            using (var hamsterDb = new HamsterDb())
            {
                //Laddar in listan för varje avdelning
                var queue = hamsterDb.Kö.Find(CurrentQueue.Id);
                hamsterDb.Entry(queue).Collection(x => x.HamsterIKö).Load();
                foreach (var hamster in queue.HamsterIKö)
                {
                    hamster.MotionsNivå += ExerciseRandomizer(1); //1 för kö
                    hamsterDb.SaveChanges();
                }
                var cage = hamsterDb.Bur.Find(CurrentCage.Id);
                hamsterDb.Entry(cage).Collection(x => x.BurHamster).Load();
                foreach (var hamster in cage.BurHamster)
                {
                    hamster.MotionsNivå += ExerciseRandomizer(2); //2 för bur
                    hamsterDb.SaveChanges();
                }
                var activity = hamsterDb.Aktivitet.Find(CurrentActivity.Id);
                hamsterDb.Entry(activity).Collection(x => x.AktivitetHamster).Load();
                foreach (var hamster in activity.AktivitetHamster)
                {
                    hamster.MotionsNivå += ExerciseRandomizer(3); //3 för aktivitet
                    hamsterDb.SaveChanges();
                }
                var hemfärdhamster = hamsterDb.Hemfärd.Find(CurrentHomecoming.Id);
                hamsterDb.Entry(hemfärdhamster).Collection(x => x.HemfärdHamster).Load();
                foreach (var hamster in hemfärdhamster.HemfärdHamster)
                {
                    hamster.MotionsNivå += ExerciseRandomizer(4); //4 för hemfärd
                    hamsterDb.SaveChanges();
                }

                var exercisehamster = hamsterDb.ResterandeHamster.Find(CurrentExercise.Id);
                hamsterDb.Entry(exercisehamster).Collection(x => x.RestHamster).Load();
                foreach (var hamster in exercisehamster.RestHamster)
                {
                    hamster.MotionsNivå += ExerciseRandomizer(4); //5 för hamstrar som e kvar
                    hamsterDb.SaveChanges();
                }
            }
        }
        //Metod som uppdaterar motionsnivån för rätt avdelning
        public int ExerciseRandomizer(int station)
        {
            int exerciseChange = 0;
            Random rd = new Random();
            if (station == 1) //kö
            {
                int odds = rd.Next(1, 100);
                if (odds <= 50)
                {
                    exerciseChange = 0;
                }
                if (odds >= 50 && odds <= 85)
                {
                    exerciseChange = 0;
                }
                if (odds > 85 && odds <= 95)
                {
                    exerciseChange = 0;
                }
                if (odds > 95)
                {
                    exerciseChange = 0;

                }
            }
            else if (station == 2) //bur
            {
                int odds = rd.Next(1, 100);
                if (odds <= 25)
                {
                    exerciseChange = 0;
                }
                if (odds > 25 && odds <= 85)
                {
                    exerciseChange = 2;
                }
                if (odds > 85 && odds <= 95)
                {
                    exerciseChange = -2;
                }
                if (odds > 95)
                {
                    exerciseChange = -4;
                }

            }
            else if (station == 3) //aktivitet
            {
                int odds = rd.Next(1, 100);
                if (odds <= 55)
                {
                    exerciseChange = -1;
                }
                if (odds > 55 && odds <= 85)
                {
                    exerciseChange = -2;
                }
                if (odds > 85 && odds <= 95)
                {
                    exerciseChange = 6;
                }
                if (odds > 95)
                {
                    exerciseChange = 8;
                }

            }
            else if (station == 4)//hemfärd/kvar i aktivitet
            {
                int odds = rd.Next(1, 100);
                if (odds <= 65)
                {
                    exerciseChange = 1;
                }
                if (odds > 65 && odds <= 85)
                {
                    exerciseChange = 2;
                }
                if (odds > 85 && odds <= 95)
                {
                    exerciseChange = 6;
                }
                if (odds > 95)
                {
                    exerciseChange = 8;
                }
            }
            return exerciseChange;
        }
        //Flyttar på hamstrarna beroende på hur många gånger de motionerat till respektive avdelning
        public void LogicForHomeOrStillExercising()
        {
            using (var hamsterDb = new HamsterDb())
            {
                //Vi laddar in alla listor för  att se om hamstern har motionerat eller ej
                var queue = hamsterDb.Kö.Find(CurrentQueue.Id);
                var home = hamsterDb.Hemfärd.Find(CurrentHomecoming.Id);
                var exercisingStill = hamsterDb.ResterandeHamster.Find(CurrentExercise.Id);
                var activity = hamsterDb.Aktivitet.Find(CurrentActivity.Id);
                var cage = hamsterDb.Bur.Find(CurrentCage.Id);
                hamsterDb.Entry(queue).Collection(x => x.HamsterIKö).Load();
                hamsterDb.Entry(home).Collection(x => x.HemfärdHamster).Load();
                hamsterDb.Entry(exercisingStill).Collection(x => x.RestHamster).Load();
                hamsterDb.Entry(activity).Collection(x => x.AktivitetHamster).Load();
                hamsterDb.Entry(cage).Collection(x => x.BurHamster).Load();
                
                // Foreach för varje avdelning för att se hamsterns motionsstatus och vilka hamstrar som ska flyttas vart
                foreach (Hamster hamster in queue.HamsterIKö.ToList())
                {
                    //Här lägger vi till hamster i bur från kö och tar bort från kö
                    //sedan raisar vi eventet om flytten
                    // samma med alla loopar och itterationer nedanför
                    if (hamster.MotionsNivå == 0)
                    {
                        cage.BurHamster.Add(hamster);          
                        queue.HamsterIKö.Remove(hamster); 
                        hamsterDb.SaveChanges();
                        
                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar HamsterMovedEvent att flytt skett.
                        HamstersHomecoming++; 
                    }
                    //if (hamster.MotionsNivå >= 1)
                    //{
                    //    activity.AktivitetHamster.Add(hamster);
                    //    cage.BurHamster.Remove(hamster);
                    //    hamsterDb.SaveChanges();
                    //    OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar HamsterMovedEvent att flytt skett.
                    //    HamstersHomecoming++;
                    //}

                    //if (hamster.MotionsNivå >= 10)
                    //{
                    //    exercisingStill.RestHamster.Add(hamster);
                    //    activity.AktivitetHamster.Add(hamster);
                    //    hamsterDb.SaveChanges();

                    //    OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar HamsterMovedEvent att flytt skett.
                    //    HamstersHomecoming++;
                    //}

                }
                foreach (Hamster hamster in cage.BurHamster.ToList())
                {
                    if (hamster.MotionsNivå == 0)
                    {
                        exercisingStill.RestHamster.Add(hamster);
                        cage.BurHamster.Remove(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar HamsterMovedEvent att flytt skett.
                        HamstersHomecoming++;
                    }

                    if (hamster.MotionsNivå >= 1)
                    {
                        activity.AktivitetHamster.Add(hamster);
                        cage.BurHamster.Remove(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar HamsterMovedEvent att flytt skett.
                        HamstersHomecoming++;
                    }

                    //if (hamster.MotionsNivå >= 10)
                    //{
                    //    home.HemfärdHamster.Add(hamster);
                    //    cage.BurHamster.Remove(hamster);
                    //    hamsterDb.SaveChanges();


                    //    OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar HamsterMovedEvent att flytt skett.
                    //    HamstersHomecoming++;
                    //}


                }
                foreach (Hamster hamster in activity.AktivitetHamster.ToList())
                {
                    if (hamster.MotionsNivå == 0)
                    {
                        exercisingStill.RestHamster.Add(hamster);
                        activity.AktivitetHamster.Remove(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar HamsterMovedEvent att flytt skett.
                        HamstersHomecoming++;
                    }
                    if (hamster.MotionsNivå >= 2)
                    {
                        home.HemfärdHamster.Add(hamster);
                        activity.AktivitetHamster.Remove(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar HamsterMovedEvent att flytt skett.
                        HamstersHomecoming++;
                    }
                    //if (hamster.MotionsNivå >= 1)
                    //{
                    //    home.HemfärdHamster.Add(hamster);
                    //    exercisingStill.RestHamster.Remove(hamster);
                    //    hamsterDb.SaveChanges();
                    //    OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar HamsterMovedEvent att flytt skett.
                    //    HamstersHomecoming++;
                    //}

                }
                foreach (Hamster hamster in exercisingStill.RestHamster.ToList())
                {
                    if (exercisingStill.RestHamster.Count > 0)
                    {
                        home.HemfärdHamster.Add(hamster);
                        exercisingStill.RestHamster.Remove(hamster);
                        hamsterDb.SaveChanges();


                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar HamsterMovedEvent att flytt skett.
                        HamstersHomecoming++;
                    }
                }

            }

        }
    }


}

