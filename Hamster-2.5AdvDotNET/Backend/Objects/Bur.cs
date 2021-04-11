using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Objects
{
    public class Bur
    {
        public int Id { get; set; }
        public virtual List<Hamster> BurHamster { get; set; }
        public Bur()
        {
            this.BurHamster = new List<Hamster>();
        }
        public override string ToString()
        {
            return "Bur";
        }
    }
}
