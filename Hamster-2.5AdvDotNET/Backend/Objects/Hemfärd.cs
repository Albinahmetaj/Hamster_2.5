using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Objects
{
    public class Hemfärd
    {
        public int Id { get; set; }
        public virtual List<Hamster> HemfärdHamster { get; set; }
        public Hemfärd()
        {
            this.HemfärdHamster = new List<Hamster>();
        }
    }
}
