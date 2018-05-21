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
        Task<JournalSearchResult> GetModelListAsync(long? id, long? toId, long? typeId, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<JournalSearchResult> GetModelListAsync(string typeName, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<JournalDTO[]> GetUserModelListAsync(long id);
        Task<JournalDTO[]> GetMerchantModelListAsync(long id);
    }
    public class JournalSearchResult
    {
        public JournalDTO[] Journals { get; set; }
        public long TotalCount { get; set; }
        public long? GivingIntegrals { get; set; }
        public long? UseIntegrals { get; set; }
        public long? GivingIntegralCount { get; set; }
        public long? UseIntegralCount { get; set; }
    }
}
