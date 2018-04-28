using Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Config
{
    class NavBarConfig:EntityTypeConfiguration<NavBarEntity>
    {
        public NavBarConfig()
        {
            ToTable("T_NavBars");
        }
    }
}
