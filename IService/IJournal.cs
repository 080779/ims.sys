using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IJournal
    {
        bool Add(long userId, long toUserId, long Integral, string typeName, string toTypeName, string description);
    }
}
