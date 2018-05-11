using IMS.Common;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{   
    [AllowAnonymous]
    public class LoginController : Controller
    {
        public IAdminService adminService { get; set; }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(string mobile,string password)
        {
            if(string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 1, Msg = "管理员用户名不能为空" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 1, Msg = "管理员密码不能为空" });
            }
            long res=await adminService.CheckLogin(mobile, password);
            if (res<=0)
            {
                return Json(new AjaxResult { Status = 1, Msg = "管理员用户名或密码错误" });
            }
            Session["Platform_AdminUserId"] = res;
            return Json(new AjaxResult { Status = 302, Msg = "登录成功", Data = "/admin/home/index" });
        }
    }
}