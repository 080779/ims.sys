using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class JournalConfig:EntityTypeConfiguration<JournalEntity>
    {
        public JournalConfig()
        {
            ToTable("T_Journals");
            HasRequired(j => j.PlatformUser).WithMany().HasForeignKey(j => j.PlatformUserId).WillCascadeOnDelete(false);
            HasRequired(j => j.ToPlatformUser).WithMany().HasForeignKey(j => j.ToPlatformUserId).WillCascadeOnDelete(false);
            HasRequired(j => j.State).WithMany().HasForeignKey(j => j.StateId).WillCascadeOnDelete(false);
            Property(j => j.Description).HasMaxLength(100);
        }
    }
}
