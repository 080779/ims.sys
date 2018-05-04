using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class JournalUserTypeConfig : EntityTypeConfiguration<JournalUserTypeEntity>
    {
        public JournalUserTypeConfig()
        {
            ToTable("T_JournalUserTypes");
            Property(j => j.Name).HasMaxLength(30).IsRequired();
            Property(j => j.Description).HasMaxLength(100);
        }
    }
}