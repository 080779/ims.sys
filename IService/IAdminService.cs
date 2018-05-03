using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IAdminService:IServiceSupport
    {
        Task<long> AddAsync(string mobile,string description,string password,long[] permissionIds);
        Task<bool> UpdateAsync(long id, string mobile, string description, string password, long[] permissionIds);
        Task<bool> DeleteAsync(long id);
        Task<AdminDTO> GetModelAsync(long id);
        Task<AdminSearchResult> GetModelListAsync(string mobile,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
    }
    public class AdminSearchResult
    {
        public AdminDTO[] Admins { get; set; }
        public long TotalCount { get; set; }
    }
}
