using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    public class SystemController : Controller
    {
        public ActionResult AdminUsers()
        {
            return View();
        }
        public ActionResult AddUser()
        {
            return View();
        }
        public ActionResult Roles()
        {
            return View();
        }
        public ActionResult AddRole()
        {
            return View();
        }
    }
}