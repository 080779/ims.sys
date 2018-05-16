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
        private int pageSize = 10;
        public ActionResult List()
        {            
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await adminService.GetModelListAsync(mobile, startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.Admins = result.Admins;
            PermissionTypeDTO[] types = await permissionTypeService.GetModelList();
            List<PermissionType> permissionTypes = new List<PermissionType>();
            foreach (var type in types)
            {
                PermissionType permissionType = new PermissionType();
                permissionType.Name = type.Name;
                PermissionDTO[] permissions = await permissionService.GetByTypeIdAsync(type.Id);
                permissionType.Permissions = permissions.ToList();
                permissionTypes.Add(permissionType);
            }
            model.PermissionTypes = permissionTypes;

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
            model.Pages = pager.Pages;
            model.PageCount = pager.PageCount;
            return Json(new AjaxResult { Status = 1, Data = model });
        }        

        public async Task<ActionResult> Add(string mobile,string password)
        {
            if(string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 1, Msg="管理员账号不能为空"});
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 1, Msg = "管理员密码不能为空" });
            }
            long res= await adminService.AddAsync(mobile, null, password);
            if(res<=0)
            {
                return Json(new AjaxResult { Status = 1, Msg = "添加管理员失败" });
            }
            return Json(new AjaxResult { Status = 1,Msg= "添加管理员成功", Data = "/admin/admin/list" });
        }
        public async Task<ActionResult> EditPermission(long id,long[] permissionIds)
        {
            if (permissionIds==null)
            {
                permissionIds = new long[] { };
            }
            bool res = await adminService.UpdateAsync(id,permissionIds);
            if(!res)
            {
                return Json(new AjaxResult { Status = 1, Msg = "编辑管理员权限失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "编辑管理员权限成功", Data = "/admin/admin/list" });
        }
        public async Task<ActionResult> EditPassword(long id, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 1, Msg = "请选择权限项" });
            }
            bool res = await adminService.UpdateAsync(id, password);
            if (!res)
            {
                return Json(new AjaxResult { Status = 1, Msg = "编辑管理员密码失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "编辑管理员密码成功", Data = "/admin/admin/list" });
        }
        public async Task<ActionResult> GetPerm(long id,long[] permissionIds)
        {
            if(permissionIds==null)
            {
                permissionIds = new long[] { };
            }
            PermissionTypeDTO[] types = await permissionTypeService.GetModelList();
            List<PermissionType> permissionTypes = new List<PermissionType>();
            foreach (var type in types)
            {
                PermissionType permissionType = new PermissionType();
                permissionType.Name = type.Name;
                PermissionDTO[] permissions = await permissionService.GetByTypeIdAsync(type.Id);
                foreach(var perm in permissions)
                {
                    if(permissionIds.Contains(perm.Id))
                    {
                        perm.IsChecked = true;
                    }
                }
                permissionType.Permissions = permissions.ToList();
                permissionTypes.Add(permissionType);
            }
            return Json(new AjaxResult { Status = 1, Data = permissionTypes });
        }
        public async Task<ActionResult> Frozen(long id)
        {
            bool res= await adminService.FrozenAsync(id);
            if(!res)
            {
                return Json(new AjaxResult { Status = 1, Msg = "冻结、解冻管理员账户操作失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "冻结、解冻管理员账户操作成功" });
        }
        public async Task<ActionResult> Del(long id)
        {
            bool res = await adminService.DeleteAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 1, Msg = "删除管理员账户失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除管理员账户成功" });
        }
    }
}