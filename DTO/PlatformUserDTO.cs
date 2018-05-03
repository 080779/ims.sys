using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class PlatformUserDTO:BaseDTO
    {
        public string Mobile { get; set; }
        public string UserCode { get; set; }
        public string Description { get; set; }
        public decimal GivingAmount { get; set; }
        public decimal UseAmount { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public int ErrorCount { get; set; }
        public DateTime ErrorTime { get; set; }
        public bool IsEnabled { get; set; }
        public long PlatformUserTypeId { get; set; }
        public long IntegralTypeId { get; set; }
        public string  PlatformUserTypeName { get; set; }
        public  string IntegralTypeName { get; set; }
    }
}
