using IMS.Common;
using IMS.IService;
using IMS.Web.Areas.Admin.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        public IAdminService adminService { get; set; }
        private int pageSize = 3;
        public async Task<ActionResult> List(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await adminService.GetModelListAsync(mobile, startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.Admins = result.Admins;

            Pagination pager = new Pagination();
            pager.PageIndex = pageIndex;
            pager.PageSize = pageSize;
            pager.TotalCount = result.TotalCount;

            if (result.TotalCount <= pageSize)
            {
                model.PageHtml = "";
            }
            else
            {
                model.PageHtml = pager.GetPagerHtml();
            }
            return View(model);
        }
    }
}