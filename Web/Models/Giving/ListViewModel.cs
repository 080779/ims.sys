using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Giving
{
    public class ListViewModel
    {
        public JournalDTO[] Journals { get; set; }
        public string PageHtml { get; set; }
    }
}