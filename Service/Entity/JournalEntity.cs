using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 流水实体类
    /// </summary>
    public class JournalEntity:BaseEntity
    {
        public long PlatformUserId { get; set; }
        public long ToPlatformUserId { get; set; }
        public long JournalTypeId { get; set; }
        public long JournalUserTypeId { get; set; }
        public long ChangeTypeId { get; set; }
        public long IntegralTypeId { get; set; }
        public string Description { get; set; }
        public long? InIntegral { get; set; }
        public long? OutIntegral { get; set; }
        public long? Integral { get; set; }
        public decimal? Amount { get; set; }
        public virtual JournalTypeEntity JournalType { get; set; }
        public virtual JournalUserTypeEntity JournalUserType { get; set; }
        public virtual PlatformUserEntity PlatformUser { get; set; }
        public virtual PlatformUserEntity ToPlatformUser { get; set; }
        public virtual ChangeTypeEntity ChangeType { get; set; }
        public virtual IntegralTypeEntity IntegralType { get; set; }
    }
}
