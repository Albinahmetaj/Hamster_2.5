using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Objects
{
    public class HamsterKö
    {
        public int Id { get; set; }
        public virtual List<Hamster> HamsterIKö { get; set; }
        public HamsterKö()
        {
            this.HamsterIKö = new List<Hamster>();
        }
        public override string ToString()
        {
            return "Kö";
        }
    }
}
