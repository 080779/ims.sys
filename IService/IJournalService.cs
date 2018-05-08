using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IJournalService:IServiceSupport
    {
        Task<JournalSearchResult> GetModelList(long? id, long? typeId, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class JournalSearchResult
    {
        public JournalDTO[] Journals { get; set; }
        public long TotalCount { get; set; }
        public long? GivingIntegrals { get; set; }
        public long? UseIntegrals { get; set; }
    }
}
