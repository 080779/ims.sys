﻿using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Admin.Models.Admin
{
    public class ListViewModel
    {
        public AdminDTO[] Admins { get; set; }
        public string PageHtml { get; set; }
        public List<PermissionType> PermissionTypes { get; set; }
    }
    public class PermissionType
    {
        public string Name { get; set; }
        public List<PermissionDTO> Permissions { get; set; }
    }
}