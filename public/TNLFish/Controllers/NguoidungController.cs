using System;
using System.Linq;
using System.Web.Mvc;
using TNLFish.Models;
using TNLFish.common;
using Facebook;
using System.Configuration;
using System.Text;
using ASPSnippets.GoogleAPI;
using System.Web.Script.Serialization;

namespace TNLFish.Controllers
{
    public class NguoidungController : Controller
    {
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        // GET: Nguoidung
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KHACHHANG khach)
        {
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var mk = Encryptor.MD5Hash(collection["MatKhau"]);
            var xnmk = Encryptor.MD5Hash(collection["XNMatKhau"]);
            var email = collection["Email"];
            var diachi = collection["DiaChi"];
            var dt = collection["DienThoai"];
            var ngaysinh = string.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);

            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được để trống!";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập tên đăng nhập!";
            }
            else if (String.IsNullOrEmpty(mk))
            {
                ViewData["Loi3"] = "Phải nhập mật khẩu!";
            }
            else if (String.IsNullOrEmpty(xnmk))
            {
                ViewData["Loi4"] = "Phải nhập lại mật khẩu!";
            }
            else if (xnmk != mk)
            {
                ViewData["Loi4"] = "Mật khẩu xác nhận không đúng !";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Phải nhập Email!";
            }
            else if (String.IsNullOrEmpty(dt))
            {
                ViewData["Loi6"] = "Phải nhập số điện thoại!";
            }
            else if (String.IsNullOrEmpty(ngaysinh))
            {
                ViewData["Loi7"] = "Phải nhập ngày sinh!";
            }
            else
            {
                khach.HoTen = hoten;
                khach.Taikhoan = tendn;
                khach.Matkhau = mk;
                khach.DiachiKH = diachi;
                khach.Email = email;
                khach.DienthoaiKH = dt;
                khach.Ngaysinh = DateTime.Parse(ngaysinh);
                CommonConstants.db.KHACHHANGs.Add(khach);
                CommonConstants.db.SaveChanges();
                return RedirectToAction("Dangnhap");
            }
            return this.Dangky();
        }

        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            // gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["TenDN"];
            var mk = Encryptor.MD5Hash(collection["MatKhau"]);
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
                // gán giá trị cho đối tượng được tạo mới (kh)
                KHACHHANG kh = CommonConstants.db.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn && n.Matkhau == mk);
                if (kh != null)
                {
                    ViewBag.Thongbao = "Chúc mừng đăng nhập thành công!";
                    Session["Taikhoan"] = kh;
                    Session["User"] = kh.Taikhoan;
                    return RedirectToAction("Index", "Home");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!";
            }
            return View();
        }

        public ActionResult Dangxuat()
        {
            Session["Taikhoan"] = null;
            return Redirect("/");
        }

        // Login with Facebook
        public ActionResult LoginFb()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code, KHACHHANG khach)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                // Lấy thông tin người dùng facebook
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string userName = me.email;
                string email = me.email;
                string fisrtName = me.fisrt_name;
                string middleName = me.middle_name;
                string lastName = me.last_name;

                // Kiểm tra xem khachhang đã có tài khoản chưa
                int slkhach = CommonConstants.db.KHACHHANGs.Count(x => x.Taikhoan == userName);
                if(slkhach == 0)
                {   // Tạo tài khoản mới cho khachhang
                    // Lưu hoten khachhang
                    StringBuilder hoten = new StringBuilder();
                    string[] names = { fisrtName, middleName, lastName };
                    foreach (var name in names)
                    {
                        if(name != null)
                        {
                            hoten.Append(name);
                        }
                    }
                    // Lưu các thông số tài khoản khachhang
                    khach.HoTen = hoten.ToString();
                    khach.Taikhoan = userName;
                    khach.Email = email;
                    khach.Matkhau = "";
                    khach.DiachiKH = "";
                    khach.DienthoaiKH = "";
                    CommonConstants.db.KHACHHANGs.Add(khach);
                    CommonConstants.db.SaveChanges();

                    // Lấy tài khoản mới được tạo
                    khach = CommonConstants.db.KHACHHANGs.SingleOrDefault(x => x.Taikhoan == userName);
                    // gán giá trị cho đối tượng được tạo mới (kh)
                    ViewBag.Thongbao = "Chúc mừng đăng nhập thành công!";
                    Session["Taikhoan"] = khach;
                    Session["User"] = khach.Taikhoan;
                }
                else if(slkhach > 0)
                {
                    khach = CommonConstants.db.KHACHHANGs.SingleOrDefault(x => x.Taikhoan == userName);
                    // gán giá trị cho đối tượng
                    ViewBag.Thongbao = "Chúc mừng đăng nhập thành công!";
                    Session["Taikhoan"] = khach;
                    Session["User"] = khach.Taikhoan;
                }
            }
            return RedirectToAction("Index", "Home");
        }

        // Login with Google Plus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void LoginWithGooglePlus()
        {
            // Gán giá trị Client ID và Client Secret qua 2 thuộc tính ClientID và ClientSecret của class GoogleConnect
            GoogleConnect.ClientId = ConfigurationManager.AppSettings["ClientId"];
            GoogleConnect.ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            // Gán URL chuyển hướng qua thuộc tính RedirectUri
            // Giá trị này chắc chắn sẽ có giá trị giống như giá trị đã thiết lập cho Authorized redirect URIs ở trên thông qua method Split
            // Lấy giá trị động thay vì truyền giá trị tĩnh
            GoogleConnect.RedirectUri = Request.Url.AbsoluteUri.Split('?')[0];
            // Method này yêu cầu chúng ta phải gửi kèm giá trị ClientID, ClientSecret, RedirectUri ở trên
            // để yêu cầu quyền lấy thông tin tiểu sử (profile) và email từ người dùng
            GoogleConnect.Authorize("profile", "email");
        }

        // Action xử lý lưu thông tin người dùng từ Google API truyền về
        [ActionName("LoginWithGooglePlus")]
        public ActionResult LoginWithGooglePlusConfirmed(KHACHHANG khach)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["code"]))
            {
                string code = Request.QueryString["code"];
                string json = GoogleConnect.Fetch("me", code);
                GoogleProfile profile = new JavaScriptSerializer().Deserialize<GoogleProfile>(json);

                if (CommonConstants.db.KHACHHANGs.SingleOrDefault(acc => acc.Taikhoan == profile.Name) != null)
                {
                    khach = CommonConstants.db.KHACHHANGs.SingleOrDefault(acc => acc.Taikhoan == profile.Name);
                    // gán giá trị cho đối tượng
                    ViewBag.Thongbao = "Chúc mừng đăng nhập thành công!";
                    Session["Taikhoan"] = khach;
                    Session["User"] = khach.Taikhoan;
                    return RedirectToAction("Index", "Home");
                }
                else if (CommonConstants.db.KHACHHANGs.SingleOrDefault(mail => mail.Email == profile.Email) != null)
                {
                    khach = CommonConstants.db.KHACHHANGs.SingleOrDefault(mail => mail.Email == profile.Email);
                    // gán giá trị cho đối tượng
                    ViewBag.Thongbao = "Chúc mừng đăng nhập thành công!";
                    Session["Taikhoan"] = khach;
                    Session["User"] = khach.Taikhoan;
                    return RedirectToAction("Index", "Home");
                }

                // Tạo tài khoản khachhang mới nếu chưa có tài khoản khachhang
                /*
                khach = new KHACHHANG()
                {
                    Taikhoan = profile.Name,
                    HoTen = profile.Name,
                    Email = profile.Email,
                    Matkhau = "",
                    DiachiKH = "",
                    DienthoaiKH = ""
                };
                */
                // Lấy thông tin người dùng từ biến profile
                khach.Taikhoan = profile.Name;
                khach.HoTen = profile.Name;
                khach.Email = profile.Email;
                khach.Matkhau = "";
                khach.DiachiKH = "";
                khach.DienthoaiKH = "";
                CommonConstants.db.KHACHHANGs.Add(khach);
                CommonConstants.db.SaveChanges();
                // Lấy tài khoản mới được tạo
                khach = CommonConstants.db.KHACHHANGs.SingleOrDefault(acc => acc.Taikhoan == profile.Name);
                // gán giá trị cho đối tượng được tạo mới (kh)
                ViewBag.Thongbao = "Chúc mừng đăng nhập thành công!";
                Session["Taikhoan"] = khach;
                Session["User"] = khach.Taikhoan;
            }

            // Kiểm tra nếu người dùng không cho phép ứng dụng truy cập thông tin của họ
            // thì chúng ta sẽ trả về dòng báo lỗi access_denied
            if (Request.QueryString["error"] == "access_denied")
            {
                return Content("access_denied");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}