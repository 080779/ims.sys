using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Journal
{
    public class PlatformAdd : IJournal
    {
        public bool Add(long userId, long toUserId, long Integral, string typeName, string toTypeName, string description)
        {
            throw new NotImplementedException();
        }
    }
}
