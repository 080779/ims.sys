using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class JournalTypeConfig:EntityTypeConfiguration<JournalTypeEntity>
    {
        public JournalTypeConfig()
        {
            ToTable("T_JournalTypes");
            Property(j => j.Name).HasMaxLength(30).IsRequired();
            Property(j => j.Description).HasMaxLength(100);
        }
    }
}
