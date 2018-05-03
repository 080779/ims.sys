using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class JournalDTO:BaseDTO
    {
        public long PlatformUserId { get; set; }
        public long ToPlatformUserId { get; set; }
        public long JournalTypeId { get; set; }
        public long ChangeTypeId { get; set; }
        public long IntegralTypeId { get; set; }
        public long StateId { get; set; }
        public string Description { get; set; }
        public decimal? InAmount { get; set; }
        public decimal? OutAmount { get; set; }
        public decimal? Amount { get; set; }
        public string JournalTypeName { get; set; }
        public string PlatformUserMobile { get; set; }
        public string ToPlatformUserMobile { get; set; }
        public string ChangeTypeName { get; set; }
        public string StateName { get; set; }
    }
}
