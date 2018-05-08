using IMS.Common;
using IMS.IService;
using IMS.Web.Models.Giving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class GivingController : Controller
    {
        public IJournalService journalService { get; set; }
        public IJournalTypeService journalTypeService { get; set; }
        private int pageSize = 3;
        public async Task<ActionResult> List()
        {
            long id = Convert.ToInt64(Session["Merchant_User_Id"]);
            long? typeId = await journalTypeService.GetIdByDescAsync("赠送");
            var result = await journalService.GetModelListAsync(id, typeId, null,null,null,null,1,pageSize);
            ListViewModel model = new ListViewModel();
            model.Journals = result.Journals;

            Pagination pager = new Pagination();
            pager.PageIndex = 1;
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
            //return Json(new AjaxResult { Status = 1, Data = model });
            return View(model);
        }
    }
}