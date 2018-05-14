﻿using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Journal
{
    public class ListViewModel
    {
        public JournalDTO[] Journals { get; set; }
        public string PageHtml { get; set; }
        public List<Page> Pages { get; set; }
        public long PageCount { get; set; }
        public long? GivingIntegralCount { get; set; }
        public long? UseIntegralCount { get; set; }
    }
}