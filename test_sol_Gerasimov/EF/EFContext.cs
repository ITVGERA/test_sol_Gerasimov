using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace test_sol_Gerasimov.EF
{
    class EFContext:DbContext
    {
        public EFContext(string name) : base(name)
        {
            Database.SetInitializer(new DataBaseInitializer());
        }
        public DbSet<Cnews> EFnews { get; set; }
    }
}
