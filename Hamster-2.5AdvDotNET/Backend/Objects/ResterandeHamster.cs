using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Objects
{
    public class ResterandeHamster
    {
        public int Id { get; set; }
        public virtual List<Hamster> RestHamster { get; set; }

    }
}
