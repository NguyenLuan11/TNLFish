using System;
using System.Linq;
using System.Web.Mvc;
using TNLFish.common;
using TNLFish.Models;

namespace TNLFish.Controllers
{
    public class LoginAdminController : Controller
    {
        // GET: LoginAdmin
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            // Gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["username"];
            var mk = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập!";
            }
            else if (String.IsNullOrEmpty(mk))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu!";
            }
            else
            {
                // Gán giá trị cho đối tượng được tạo mới (ad)
                ADMIN ad = CommonConstants.db.ADMINs.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == mk);
                if (ad != null)
                {
                    var userSession = new UserLogin();
                    userSession.UserName = ad.UserAdmin;
                    userSession.Password = ad.PassAdmin;
                    Session.Add(CommonConstants.USER_SESSION, userSession);

                    ViewBag.Thongbao = "Chúc mừng! Đăng nhập thành công!";
                    Session["Taikhoanadmin"] = ad;
                    Session["Admin"] = ad.HoTen;
                    return RedirectToAction("Home", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!";
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["Taikhoanadmin"] = null;
            return RedirectToAction("Login", "LoginAdmin");
        }
    }
}