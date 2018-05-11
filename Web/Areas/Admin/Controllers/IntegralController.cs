using IMS.Common;
using IMS.IService;
using IMS.Web.Areas.Admin.Models.Integral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class IntegralController : Controller
    {
        public IJournalService journalService { get; set; }
        public IJournalTypeService journalTypeService { get; set; }
        public IPlatformUserService platformUserService { get; set; }
        private int pageSize = 1;
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(long? typeId, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var user = await platformUserService.GetModelAsync("mobile", "PlatformUser201805051709360001");
            //long? typeId = await journalTypeService.GetIdByDescAsync("赠送");
            var result = await journalService.GetModelListAsync(user.Id, typeId, null, null, startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.Journals = result.Journals;
            model.JournalTypes = await journalTypeService.GetModelListAsync(true);
            model.GivingIntegralCount = user.GivingIntegral;
            model.UseIntegralCount = user.UseIntegral;
            model.PlatformIntegral = user.PlatformIntegral;

            Pagination pager = new Pagination();
            pager.PageIndex = pageIndex;
            pager.PageSize = pageSize;
            pager.TotalCount = result.TotalCount;

            if (result.TotalCount <= pageSize)
            {
                model.PageHtml = "test";
            }
            else
            {
                model.PageHtml = pager.GetPagerHtml();
            }
            model.Pages = pager.Pages;
            model.PageCount = pager.PageCount;
            return Json(new AjaxResult { Status = 1, Data = model });
            //return View(model);
        }
    }
}