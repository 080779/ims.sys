using IMS.Common;
using IMS.IService;
using IMS.Web.Areas.Admin.Models.Merchant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class MerchantController : Controller
    {
        public IPlatformUserService platformUserService { get; set; }
        private int pageSize = 1;
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await platformUserService.GetModelListAsync(mobile, null, "商家", startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.PlatformUsers = result.PlatformUsers;

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
        }
        [HttpPost]
        public async Task<ActionResult> Add(string mobile, string code, string password,string tradePassword)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商家账号不能为空" });
            }
            if (string.IsNullOrEmpty(code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "会员编号不能为空" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "登录密码不能为空" });
            }
            if (string.IsNullOrEmpty(tradePassword))
            {
                return Json(new AjaxResult { Status = 0, Msg = "交易密码不能为空" });
            }
            if (await platformUserService.IsExist("mobile", mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商家账号已经存在" });
            }
            if (await platformUserService.IsExist("code", code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "会员编号已经存在" });
            }
            if (await platformUserService.AddAsync("客户", mobile, code, password, tradePassword) <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加商家失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加商家成功" });
        }
        public async Task<ActionResult> Del(long id)
        {
            if (!await platformUserService.Del(id))
            {
                return Json(new AjaxResult { Status = 1, Msg = "删除失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除成功" });
        }
        public async Task<ActionResult> Frozen(long id)
        {
            if (!await platformUserService.Frozen(id))
            {
                return Json(new AjaxResult { Status = 1, Msg = "冻结失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "冻结成功" });
        }
    }
}