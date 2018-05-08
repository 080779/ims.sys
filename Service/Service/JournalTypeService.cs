using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class JournalTypeService : IJournalTypeService
    {
        public async Task<long?> GetIdByDescAsync(string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var type = await dbc.GetAll<JournalTypeEntity>().SingleOrDefaultAsync(j=>j.Description==description);
                if(type==null)
                {
                    return null;
                }
                return type.Id;
            }
        }
    }
}
