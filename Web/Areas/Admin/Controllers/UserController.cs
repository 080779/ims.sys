using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Start.Filter;

namespace Web.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Login(Message model)
        {
            return View(model);
        }
        public class Message
        {
            public string Msg { get; set; }
        }
        [HttpPost]
        public ActionResult Login(long id)
        {
            Session["AdminUserId"] = id;
            return Json(new AjaxResult { Status="1"});
        }
        public ActionResult Logout()
        {
            Session["AdminUserId"] = null;
            return Json(new AjaxResult { Status = "1" });
        }
    }
}