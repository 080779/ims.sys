using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        [Route("Login")]
        public ActionResult Login()
        {
            return View();
        }
        [Route("Logout")]
        public ActionResult Logout()
        {
            return Content("tviu");
        }
    }
}