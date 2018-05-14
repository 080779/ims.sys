using IMS.Common;
using IMS.IService;
using IMS.Web.Areas.Admin.Models.TakeCash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class TakeCashController : Controller
    {
        public ITakeCashService takeCashService { get; set; }
        public IStateService stateService { get; set; }
        public IPlatformUserService platformUserService { get; set; }
        private int pageSize = 1;
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(long? stateId, string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            //long? typeId = await journalTypeService.GetIdByDescAsync("赠送");
            TakeCashSearchResult result = await takeCashService.GetModelListAsync(stateId,mobile,startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.TakeCashes = result.TakeCashes;
            model.States = await stateService.GetModelListAsync();
            model.GivingIntegralCount = result.GivingIntegralCount;
            model.UseIntegralCount = result.UseIntegralCount;

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