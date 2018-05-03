using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
    }
}