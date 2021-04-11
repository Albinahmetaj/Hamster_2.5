using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Objects
{
    public class Aktivitet
    {
        public int Id { get; set; }
        public virtual List<Hamster> AktivitetHamster { get; set; }
        public Aktivitet()
        {
            this.AktivitetHamster = new List<Hamster>();
        }
        public override string ToString()
        {
            return "Aktivitet";
        }
    }
}
