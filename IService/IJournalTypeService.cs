using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IJournalTypeService:IServiceSupport
    {
        Task<long?> GetIdByDescAsync(string description);
    }
}
