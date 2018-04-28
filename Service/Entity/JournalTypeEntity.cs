﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 流水类型实体类
    /// </summary>
    public class JournalTypeEntity:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
