using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.Admin.Models.NavBar
{
    public class AddChildModel
    {
        public string MenuName { get; set; }
        public string Url { get; set; }
        public long Sort { get; set; }
        public long ParentId { get; set; }
    }
}