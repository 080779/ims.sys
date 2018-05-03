using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class ChangeTypeConfig:EntityTypeConfiguration<ChangeTypeEntity>
    {
        public ChangeTypeConfig()
        {
            ToTable("T_ChangeTypes");
            Property(c => c.Name).HasMaxLength(30).IsRequired();
            Property(c => c.Description).HasMaxLength(100);
        }       
    }
}
