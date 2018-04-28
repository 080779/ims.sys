using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        public ActionResult Login(string msg="")
        {
            return View((object)msg);
        }
    }
}