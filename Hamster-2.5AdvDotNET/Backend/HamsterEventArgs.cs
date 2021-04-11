using Backend.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
        public class HamsterEventArgs : EventArgs
        {
            public Hamster Hamster { get; set; }
            public HamsterKö QueueEvent { get; set; }
            public Bur CageEvent { get; set; }
            public Aktivitet ActivityEvent { get; set; }
            public Hemfärd HomeEvent { get; set; }

            public ResterandeHamster ExercisingEvent { get; set; }

            public HamsterEventArgs(Hamster hamster)
            {
                this.Hamster = hamster;
            }
        }
    }
