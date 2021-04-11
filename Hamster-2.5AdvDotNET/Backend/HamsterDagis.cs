using Backend.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    public class HamsterDagis
    {
        //Ändra Denna variabel för att köra mot mer eller mindre patienter.
        public int AmountToSimulate = 30;
        //Variabel för att styra hela simuleringen. Körs till Dismisspatients är > NumberOfPatientsToSimulate.
        public int HamstersHomecoming = 0;
        //Skapar en instans av alla sjukhusavdelnignar för att lätt kunna ladda in de aktualla avdelningarna från databasen.
        //Hittar alltid därför rätt avdelning i databasen med hjälp av tex CurrentQueue.Id.

        static HamsterKö CurrentQueue = new HamsterKö();
        static Bur CurrentCage = new Bur();
        static Aktivitet CurrentActivity = new Aktivitet();
        static Hemfärd CurrenyHomecoming = new Hemfärd();
        static ResterandeHamster CurrentExercise = new ResterandeHamster();

        public event EventHandler<HamsterEventArgs> HamsterMovedEvent;
        /// <summary>
        /// EventRaiser. Kallar på denna när hospital skall raisa ett event.
        /// </summary>
        /// <param name="e"></param>
        /// 

        internal virtual void OnHamsterMoved(HamsterEventArgs e)
        {
            HamsterMovedEvent?.Invoke(this, e);
        }
        /// <summary>
        /// Lägger till 30 patienter. Ändra på NumberOfPatientsToSimulate om man vill köra större simulering.
        /// </summary>
        public void AddHamster()
        {
            using (var hamsterDb = new HamsterDb())
            {
                DateTime dateTime = new DateTime(2021, 4, 11, 07, 00, 00);


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
                Console.WriteLine("Checkar in hamstrar...Dagens datum och tid är: {0}", dateTime);
                Console.WriteLine();
                //var patient = new Patient(Randomers.GenerateName(), Randomers.GenerateÄgarNamn(), Randomers.GenerateÅlder(), Randomers.GenerateKön(), Randomers.GenerateSSN(), Randomers.GenerateSymptomLevel());

                //var Patient = new Patient(Randomers.GenerateSSN(), Randomers.GenerateSymptomLevel());

                //var hamster = Program.ProcessHamstrar("Hamsterlista30.csv");
                //if (!db.Patients.Any())
                //{
                //    foreach (var item in hamster)
                //    {
                //        db.Patients.Add(item);
                //        CurrentQueue.PatientsInQueue.Add(item);
                //    }
                //}

                for (int i = 0; i < AmountToSimulate; i++)
                {

                    Thread.Sleep(200); // För att det inte ska bli dubletter av Patient. Random arbetar med clockcykler
                                       //var patient = new Patient(Randomers.GenerateName(), Randomers.GenerateÄgarNamn(), Randomers.GenerateÅlder(), Randomers.GenerateKön(), Randomers.GenerateSSN(), Randomers.GenerateSymptomLevel());
                    var hamster = new Hamster(RandomLogic.GenerateIdentity(), RandomLogic.GenerateExerciseLevel());
                    hamsterDb.Hamster.Add(hamster);
                    CurrentQueue.HamsterIKö.Add(hamster);

                }

                //sorterar databasen för att det skulle vara lättare att följa under testning osv. fyller ingen funktion för programmets funktionalitet.
                CurrentQueue.HamsterIKö = CurrentQueue.HamsterIKö.OrderByDescending(x => x.hamsterID).ThenBy(x => x.MotionsNivå).ToList(); //Sorterar före inläggning i db.
                hamsterDb.Kö.Add(CurrentQueue);
                hamsterDb.Bur.Add(CurrentCage);
                hamsterDb.Aktivitet.Add(CurrentActivity);
                hamsterDb.Hemfärd.Add(CurrenyHomecoming);
                hamsterDb.ResterandeHamster.Add(CurrentExercise);
                //Lägger till alla avdelningar i databasen så dom finns att ladda in senare.
                hamsterDb.SaveChanges();
            }
        }

        public void MoveHamster()
        {
            using (var hamsterDb = new HamsterDb())
            {
                //Laddar in rätt avdelningar från databasen.
                var queue = hamsterDb.Kö.Find(CurrentQueue.Id);
                var activity = hamsterDb.Aktivitet.Find(CurrentActivity.Id);
                var cage = hamsterDb.Bur.Find(CurrentCage.Id);


                //Laddar in avdelningarnas patienter om dom har patienter i sig.
                //Behöver inte load IVA för vi ska inte ta bort ifrån iva i detta läget.
                //if (sanatorium.Patients.Count > 0)
                //{
                //    db.Entry(sanatorium).Collection(p => p.Patients).Load();
                //    sanatorium.Patients = sanatorium.Patients.OrderByDescending(x => x.patientID).ThenBy(x => x.SymptomLevel).ToList();
                //}
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
                //Fylller på 5 patienter i IVA första varvet, sedan fyller vi på när plats finns.
                while (cage.BurHamster.Count < 3)
                {
                    int result = FindHamster();
                    Hamster hamster;
                    //Om patient kom från sanatorium
                    if (result == 0)
                    {
                        hamster = queue.HamsterIKö.FirstOrDefault();
                        cage.BurHamster.Add(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //Raisar Event att flytt skett
                        queue.HamsterIKö.Remove(hamster);
                    }
                    //Om patient kom från kön
                    else
                    {
                        hamster = queue.HamsterIKö.FirstOrDefault();
                        cage.BurHamster.Add(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //Raisar event att flytt skett
                        queue.HamsterIKö.Remove(hamster);

                    }
                }
                hamsterDb.SaveChanges();
                //Första gången lägger vi till 10 patienter i sanatorium, annars fyller vi på efterhand.
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
        /// <summary>
        /// Hittar den sjukaste patienten från kö eller sanatorium
        /// </summary>
        /// <returns>1 eller 0 som representation av vilken avdeling vi ska ta bort ifrån</returns>
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
                //Försöker ladda in kölista och sanatoriumlista
                if (activity.AktivitetHamster.Count > 0)
                {
                    hamsterDb.Entry(activity).Collection(p => p.AktivitetHamster).Load();
                    activity.AktivitetHamster = activity.AktivitetHamster.OrderByDescending(x => x.hamsterID).ThenBy(x => x.MotionsNivå).ToList();
                    hamster3 = activity.AktivitetHamster[0];
                }

                //Returnerar 0 för sanatorium och 1 för kön.
                //Hittade vi sjuk patient från båda avdelningarna jämför vi först
                //Symptomlevel och sedan Birthdate.
                if (hamster2 != null && hamster3 != null)
                {
                    if (hamster2.MotionsNivå.CompareTo(hamster3.MotionsNivå) == 0)
                    {
                        return hamster2.CheckIn < hamster3.CheckIn ? 0 : 1;
                    }
                    return hamster2.MotionsNivå > hamster3.MotionsNivå ? 0 : 1;
                }
                //Om sanatorium inte laddade är patient2 null och därför gör vi ingen jämföring ovan och returnerar direkt 0 för kö.
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
        /// <summary>
        /// Uppdaterar patienternas symptomLevelel.
        /// </summary>
        public void UpdateExerciseLevel()
        {
            using (var hamsterDb = new HamsterDb())
            {
                //Laddar in listan för varje avdelning. och för varje patient i denna lista kör vi metoden SymptomUpdater += patient.SymptomLevel.
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
                    hamster.MotionsNivå += ExerciseRandomizer(2); //1 för kö
                    hamsterDb.SaveChanges();
                }
                var activity = hamsterDb.Aktivitet.Find(CurrentActivity.Id);
                hamsterDb.Entry(activity).Collection(x => x.AktivitetHamster).Load();
                foreach (var hamster in activity.AktivitetHamster)
                {
                    hamster.MotionsNivå += ExerciseRandomizer(3); //1 för kö
                    hamsterDb.SaveChanges();
                }
                var exercising = hamsterDb.Hemfärd.Find(CurrenyHomecoming.Id);
                hamsterDb.Entry(exercising).Collection(x => x.HemfärdHamster).Load();
                foreach (var hamster in exercising.HemfärdHamster)
                {
                    hamster.MotionsNivå += ExerciseRandomizer(4);
                    hamsterDb.SaveChanges();
                }

            }
        }
        /// <summary>
        /// Uppdaterar symptomlevel för rätt department.
        /// </summary>
        /// <param name="station">kö, iva, eller sanatorium.</param>
        /// <returns>symptomförändring</returns>
        public int ExerciseRandomizer(int station)
        {
            int exerciseChange = 0;
            Random rd = new Random();
            if (station == 1) //KÖ
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
            else if (station == 2) //IVA
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
            else if (station == 3) //SANATORIUM
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
            else if (station == 4)
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
        /// <summary>
        /// Flyttar patienter till Healthy eller Afterlive om deras symptomlevel är under 0 eller 10 och uppåt.
        /// </summary>
        public void LogicForHomeOrStillExercising()
        {
            using (var hamsterDb = new HamsterDb())
            {
                //Samma här. Laddar in ALLA listor denna gången för en patient kan bli frisk eller dör VARSOMHELST i sjukhuset.
                var queue = hamsterDb.Kö.Find(CurrentQueue.Id);
                var home = hamsterDb.Hemfärd.Find(CurrenyHomecoming.Id);
                var exercisingStill = hamsterDb.ResterandeHamster.Find(CurrentExercise.Id);
                var activity = hamsterDb.Aktivitet.Find(CurrentActivity.Id);
                var cage = hamsterDb.Bur.Find(CurrentCage.Id);
                hamsterDb.Entry(queue).Collection(x => x.HamsterIKö).Load();
                hamsterDb.Entry(home).Collection(x => x.HemfärdHamster).Load();
                hamsterDb.Entry(exercisingStill).Collection(x => x.RestHamster).Load();
                hamsterDb.Entry(activity).Collection(x => x.AktivitetHamster).Load();
                hamsterDb.Entry(cage).Collection(x => x.BurHamster).Load();
                //Gör samma foreach för VARJE avdelning för att se vilka patienter som skall flyttas till healthy eller afterlife.
                foreach (Hamster hamster in queue.HamsterIKö.ToList())
                {

                    if (hamster.MotionsNivå == 0)
                    {
                        cage.BurHamster.Add(hamster);          // vi lägger till i healthy eller afterlife beroende på IF. sedan tar bort från
                        queue.HamsterIKö.Remove(hamster); // deras nuvarande plats, sen raisar event att det skett. alla loopar ser lika ut.
                        hamsterDb.SaveChanges();
                        // bara vilka avdelningar vi arbetar med som skiljer sig.
                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar event att flytt skett.
                        HamstersHomecoming++; //++ på dismissedpatients, denna variabel styr hela programmet. Dvs när alla patienter flyttats ut
                                              // ur sjukhuset. (dom ligger i afterlife eller Healthy) kommer det sluta.
                    }
                    //if (patient.SymptomLevel >= 1)
                    //{
                    //    sanatorium.Patients.Add(patient);
                    //    iva.Patients.Remove(patient);
                    //    db.SaveChanges();

                    //    OnPatientMoved(new HospitalEventArgs(patient)); //raisar event att flytt skett.
                    //    DismissedPatients++;
                    //}
                    //if(patient.SymptomLevel >= 10)
                    //{
                    //    afterLife.Patients.Add(patient);
                    //    sanatorium.Patients.Add(patient);
                    //    db.SaveChanges();

                    //    OnPatientMoved(new HospitalEventArgs(patient)); //raisar event att flytt skett.
                    //    DismissedPatients++;
                    //}

                }
                foreach (Hamster hamster in cage.BurHamster.ToList())
                {
                    if (hamster.MotionsNivå == 0)
                    {
                        exercisingStill.RestHamster.Add(hamster);
                        cage.BurHamster.Remove(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar event att flytt skett.
                        HamstersHomecoming++;
                    }

                    if (hamster.MotionsNivå >= 1)
                    {
                        activity.AktivitetHamster.Add(hamster);
                        cage.BurHamster.Remove(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar event att flytt skett.
                        HamstersHomecoming++;
                    }

                    //if (patient.SymptomLevel >= 10)
                    //{
                    //    afterLife.Patients.Add(patient);
                    //    iva.Patients.Remove(patient);
                    //    db.SaveChanges();

                    //    OnPatientMoved(new HospitalEventArgs(patient)); //raisar event att flytt skett.
                    //    DismissedPatients++;
                    //}


                }
                foreach (Hamster hamster in activity.AktivitetHamster.ToList())
                {
                    if (hamster.MotionsNivå == 0)
                    {
                        exercisingStill.RestHamster.Add(hamster);
                        activity.AktivitetHamster.Remove(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar event att flytt skett.
                        HamstersHomecoming++;
                    }
                    if (hamster.MotionsNivå >= 2)
                    {
                        home.HemfärdHamster.Add(hamster);
                        activity.AktivitetHamster.Remove(hamster);
                        hamsterDb.SaveChanges();

                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar event att flytt skett.
                        HamstersHomecoming++;
                    }
                    //if (patient.SymptomLevel >= 1)
                    //{
                    //    afterLife.Patients.Add(patient);
                    //    healthy.Patients.Remove(patient);
                    //    db.SaveChanges();

                    //    OnPatientMoved(new HospitalEventArgs(patient)); //raisar event att flytt skett.
                    //    DismissedPatients++;
                    //}

                }
                foreach (Hamster hamster in exercisingStill.RestHamster.ToList())
                {
                    if (exercisingStill.RestHamster.Count > 0)
                    {
                        home.HemfärdHamster.Add(hamster);
                        exercisingStill.RestHamster.Remove(hamster);
                        hamsterDb.SaveChanges();


                        OnHamsterMoved(new HamsterEventArgs(hamster)); //raisar event att flytt skett.
                        HamstersHomecoming++;
                    }
                }

            }

        }
    }


}

