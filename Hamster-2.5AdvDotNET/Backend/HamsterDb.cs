using Backend.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    public class HamsterDb:DbContext
    {
        public HamsterDb() : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NewHamsterDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
        {

        }
        public DbSet<Hamster> Hamster { get; set; }
        public DbSet<HamsterKö> Kö { get; set; }
        public DbSet<Bur> Bur { get; set; }
        public DbSet<Aktivitet> Aktivitet { get; set; }
        public DbSet<Hemfärd> Hemfärd { get; set; }
        public DbSet<ResterandeHamster> ResterandeHamster { get; set; }
    }
}
