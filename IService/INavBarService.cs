using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface INavBarService:IServiceSupport
    {
        Task<long> Add(long menuId, string menuName,string url,long sort);
        Task<bool> Update(long id, string menuName, string url, long sort);
        Task<long> AddChild(string menuName, string url, long sort, long parentId);
        Task<NavBarDTO> GetById(long id);
        Task<NavBarDTO[]> GetByParentId(long id);
    }
}
