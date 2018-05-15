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
        public async Task<ActionResult> TakeCash(string mobile,string strIntegral,string type="消费积分")
        {
            if(string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不能为空" });
            }
            var user= await  platformUserService.GetModelAsync("mobile", mobile);
            if(user==null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不存在" });
            }
            if(string.IsNullOrEmpty(strIntegral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须填" });
            }
            long integral;
            if (!long.TryParse(strIntegral, out integral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须是数字" });
            }
            if(integral<0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须大于零" });
            }
            bool res= await platformUserService.TakeCashApplyAsync(user.Id, integral, type, null);
            if(!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现申请失败" });
            }
            return Json(new AjaxResult { Status = 0, Msg = "变现申请成功" });
        }

        public async Task<ActionResult> Calc(string mobile, string strIntegral, string type = "消费积分")
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不能为空" });
            }
            var user = await platformUserService.GetModelAsync("mobile", mobile);
            if (user == null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不存在" });
            }
            if (string.IsNullOrEmpty(strIntegral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须填" });
            }
            long integral;
            if (!long.TryParse(strIntegral, out integral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须是数字" });
            }
            if (integral < 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须大于零" });
            }
            decimal amount=0;
            if(type=="消费积分")
            {
                if (integral > user.UseIntegral)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "账户积分不足" });
                }
                amount = await takeCashService.CalcAsync("消费积分提现比率", integral);
            }
            if (type == "商家积分")
            {
                if(integral > user.GivingIntegral)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "账户积分不足" });
                }
                amount = await takeCashService.CalcAsync("商家积分提现比率", integral);
            }
            return Json(new AjaxResult { Status = 1, Data= amount });
        }
        public async Task<ActionResult> GetIntegral(string mobile,string type = "消费积分")
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不能为空" });
            }
            var user = await platformUserService.GetModelAsync("mobile", mobile);
            if (user == null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不存在" });
            }
            long haveIntegral = 0;
            if (type == "消费积分")
            {
                haveIntegral = user.UseIntegral;
            }
            if (type == "商家积分")
            {
                haveIntegral = user.GivingIntegral;
            }
            return Json(new AjaxResult { Status = 1, Data = haveIntegral });
        }
    }
}