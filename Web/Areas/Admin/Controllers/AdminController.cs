using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.Areas.Admin.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        public IAdminService adminService { get; set; }
        public IPlatformUserService platformUserService { get; set; }
        public IPermissionService permissionService { get; set; }
        public IPermissionTypeService permissionTypeService { get; set; }
        private int pageSize = 3;
        public async Task<ActionResult> List()
        {
            var result = await adminService.GetModelListAsync(null, null, null, 1, pageSize);
            ListViewModel model = new ListViewModel();
            model.Admins = result.Admins;
            var types = await permissionTypeService.GetModelList();
            List<PermissionType> permissionTypes = new List<PermissionType>();
            foreach(var type in types)
            {
                PermissionType permissionType = new PermissionType();
                permissionType.Name = type.Name;
                var permissions = await permissionService.GetByTypeId(type.Id);
                permissionType.Permissions = permissions.ToList();
                permissionTypes.Add(permissionType);
            }
            model.PermissionTypes = permissionTypes;

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
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> GetList(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await adminService.GetModelListAsync(mobile, startTime, endTime, pageIndex, pageSize);
            GetListViewModel model = new GetListViewModel();
            model.Admins = result.Admins;

            Pagination pager = new Pagination();
            pager.PageIndex = pageIndex;
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
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        public async Task<ActionResult> test()
        {
            //await platformUserService.AddAsync("商家", "15615615612", "15615615612", "123456", "123456");
            await platformUserService.ProvideAsync(1, 2, 15, "平台积分", "商家积分", "平台发放","");
            await platformUserService.ProvideAsync(1, 2, 15, "平台积分", "消费积分", "平台发放", "");
            await platformUserService.ProvideAsync(1, 3, 15, "平台积分", "消费积分", "平台发放", "");
            //await platformUserService.TakeOut(2, 5, "消费积分", "平台扣除");
            //await platformUserService.TakeCashApply(2, 50, "商家积分", "积分变现");
            //await platformUserService.TakeCashConfirm(1);
            return View();
        }
    }
}