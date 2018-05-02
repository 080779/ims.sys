using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class HomeController : Controller
    {
        public INavBarService navBarService { get; set; }
        public async Task<ActionResult> Index()
        {
            await navBarService.Add(1,"1","1",1);
            return View();
        }
    }
}