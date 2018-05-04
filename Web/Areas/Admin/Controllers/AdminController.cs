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
        public IPlatformUserService platformUserService { get; set; }        
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
        public async Task<ActionResult> test()
        {
            //await platformUserService.AddAsync("商家", "15615615612", "15615615612", "123456", "123456");
            await platformUserService.ProvideAsync(2, 4, 50, "商家积分", "消费积分", "赠送");
            return View();
        }
    }
}