using IMS.Common;
using IMS.Common.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.App_Start.Filter
{
    public class SYSAuthorizationFilter : IAuthorizationFilter
    {
        //public IAdminService adminUserService = DependencyResolver.Current.GetService<IAdminService>();
        //public IPermissionService permissionService = DependencyResolver.Current.GetService<IPermissionService>();
        private static string per = "admin";
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var v = filterContext.HttpContext.Request.Url;
            if(v.ToString().ToLower().Contains("/admin/"))
            {
                PermissionAttribute attribute = (PermissionAttribute)filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(PermissionAttribute), false).SingleOrDefault();
                PermissionAttribute[] attributes = (PermissionAttribute[])filterContext.ActionDescriptor.GetCustomAttributes(typeof(PermissionAttribute), false);
                //var attributes = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(true);
                long? adminUserId = (long?)filterContext.HttpContext.Session["AdminUserId"];
                if (adminUserId == null)
                {
                    if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                    {
                        return;
                    }
                    if (filterContext.HttpContext.Request.IsAjaxRequest())//判断是否是ajax请求
                    {
                        filterContext.Result = new JsonNetResult { Data = new AjaxResult { Status = "401", Data = "/admin/user/Login" } };
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("/admin/user/login");
                    }
                    return;
                }
                if (attribute == null)
                {
                    if (attributes.Length <= 0)
                    {
                        return; //如果没有权限检查的attribute就返回，不进行后面的判断
                    }
                }

                if (attribute.Permission != per)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonNetResult { Data = new AjaxResult { Status = "0", Msg = "没有" + per + "这个权限" } };
                    }
                    else
                    {
                        //filterContext.Result = new ContentResult() { Content = "没有" + permissionService.GetByName(attr.Permission).Description + "这个权限" };
                        filterContext.Result = new RedirectResult("/admin/user/login?msg="+ "没有" + per + "这个权限" + v);
                    }
                    return;
                }

                foreach (var attr in attributes)
                {
                    if (attr.Permission != per)
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new JsonNetResult { Data = new AjaxResult { Status = "0", Msg = "没有" + per + "这个权限" } };
                        }
                        else
                        {
                            //filterContext.Result = new ContentResult() { Content = "没有" + permissionService.GetByName(attr.Permission).Description + "这个权限" };
                            filterContext.Result = new RedirectResult("/admin/user/login?msg=" + "没有" + per + "这个权限" + v);
                        }
                        return;
                    }
                }
            }
            //else
            //{
            //    long? UserId = (long?)filterContext.HttpContext.Session["UserId"];
            //    if (UserId == null)
            //    {
            //        if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            //        {
            //            return;
            //        }
            //        if (filterContext.HttpContext.Request.IsAjaxRequest())//判断是否是ajax请求
            //        {
            //            filterContext.Result = new JsonNetResult { Data = new AjaxResult { Status = "401", Data = "/user/Login" } };
            //        }
            //        else
            //        {
            //            filterContext.Result = new RedirectResult("/user/login?msg="+v);
            //        }
            //        return;
            //    }
            //}
        }
    }
}