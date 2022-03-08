using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace test_sol_Gerasimov.EF
{
    class DataBaseInitializer:DropCreateDatabaseIfModelChanges<EFContext>
    {
        protected override void Seed(EFContext context)
        {
            //можно реализовать, если будет нужно
        }
    }
}
