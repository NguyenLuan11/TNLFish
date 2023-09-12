using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNLFish.common;
using System.Web.Routing;

namespace TNLFish.Controllers
{
    // BaseController kiểm tra nếu: đã có tài khoản admin đăng nhập -> cho truy cập vào Admin Home
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Lấy Session user từ UserLogin
            var session = (UserLogin)Session[CommonConstants.USER_SESSION];

            // Kiểm tra đã có tài khoản Admin đăng nhập hay chưa?
            if(session == null)
            {
                // Trả về trang "Login" nếu chưa có tài khoản admin đăng nhập
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {controller = "LoginAdmin", action = "Login" }));
            }
            else
            {
                // Kiểm tra tài khoản đăng nhập có phải là admin hay không?
                if (session.UserName.ToLower() != "admin")
                {
                    // Trả về trang "Login" nếu tài khoản đăng nhập không phải là admin
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "LoginAdmin", action = "Login" }));
                }
            }
            base.OnActionExecuting(filterContext);
        }

        // Hàm hiển thị thông báo
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;

            if (type == "success")
                TempData["AlertType"] = "alert-success";

            if (type == "warning")
                TempData["AlertType"] = "alert-warning";

            if (type == "error")
                TempData["AlertType"] = "alert-danger";
        }
    }
}