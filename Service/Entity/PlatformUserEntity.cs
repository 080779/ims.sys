using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 平台实体类
    /// </summary>
    public class PlatformUserEntity : BaseEntity
    {
        public string Mobile { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public long PlatformUserTypeId { get; set; }
        public long IntegralTypeId { get; set; }
        public virtual PlatformUserTypeEntity PlatformUserType { get; set; }
        public virtual IntegralTypeEntity IntegralType { get; set; }
    }
}
