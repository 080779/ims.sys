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
            HasRequired(j => j.ChangeType).WithMany().HasForeignKey(j => j.ChangeTypeId).WillCascadeOnDelete(false);
            HasRequired(j => j.IntegralType).WithMany().HasForeignKey(j => j.IntegralTypeId).WillCascadeOnDelete(false);
            HasRequired(j => j.JournalType).WithMany().HasForeignKey(j => j.JournalTypeId).WillCascadeOnDelete(false);
            HasRequired(j => j.JournalUserType).WithMany().HasForeignKey(j => j.JournalUserTypeId).WillCascadeOnDelete(false);
            Property(j => j.Description).HasMaxLength(100);
        }
    }
}
