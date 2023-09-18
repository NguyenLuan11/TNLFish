using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNLFish.Models;
using PagedList;
using System.IO;
using TNLFish.common;
using System.Web.WebPages;
using System.Web.UI.WebControls;

namespace TNLFish.Controllers
{
    // HomeController kế thừa BaseController
    public class AdminController : BaseController
    {
        // GET: Admin
        public ActionResult Home()
        {
            return View();
        }

        // QLdongca

        public ActionResult QLdongca()
        {
            var dongca = from dong_ca in CommonConstants.db.dong_ca select dong_ca;
            return View(dongca);
        }

        // QLloaica

        public ActionResult QLloaica(int ?page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            var loaica = CommonConstants.db.loai_ca.ToList();
            return View(loaica.OrderBy(n => n.id).ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Themloaica()
        {
            // Đưa dữ liệu vào DropdownList
            // Lấy ds từ table dong_ca, sắp xếp tăng dần theo tên dòng cá, chọn lấy giá trị MaCa, hiển thị TenDongCa
            ViewBag.MaCa = new SelectList(CommonConstants.db.dong_ca.ToList().OrderBy(n => n.TenDongCa), "MaCa", "TenDongCa");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themloaica(loai_ca loaica, HttpPostedFileBase fileUpload)
        {
            // Đưa dữ liệu vào DropdownList
            ViewBag.MaCa = new SelectList(CommonConstants.db.dong_ca.ToList().OrderBy(n => n.TenDongCa), "MaCa", "TenDongCa");
            // Kiểm tra đường dẫn file
            if(fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa!";
                return View();
            }
            // Thêm vào CSDL
            else
            {
                if(ModelState.IsValid)
                {
                    // Lưu tên file, lưu ý bổ sung thư viện System.IO
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    // Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/img"), fileName);
                    // Kiểm tra hình ảnh tồn tại chưa
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại!";
                    }
                    else
                    {
                        // Lưu hình ảnh vào đường dẫn
                        fileUpload.SaveAs(path);
                    }
                    loaica.Image = fileName;
                    // Lưu vào CSDL
                    CommonConstants.db.loai_ca.Add(loaica);
                    CommonConstants.db.SaveChanges();
                }
            }
            return RedirectToAction("Qlloaica");
        }

        // Hiển thị sản phẩm
        public ActionResult ChitietLoaica(int id)
        {
            // Lấy ra đối tượng sách theo mã
            loai_ca loaica = CommonConstants.db.loai_ca.SingleOrDefault(n => n.id == id);
            ViewBag.id = loaica.id;
            if(loaica == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loaica);
        }

        // Xóa sản phẩm
        [HttpGet]
        public ActionResult XoaLoaica(int id)
        {
            // Lấy ra đối tượng sách cần xóa theo mã
            loai_ca loaica = CommonConstants.db.loai_ca.SingleOrDefault(n => n.id == id);
            ViewBag.id = loaica.id;
            if (loaica == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loaica);
        }

        [HttpPost, ActionName("XoaLoaica")]
        public ActionResult Xacnhanxoa(int id)
        {
            // Lấy ra đối tượng sách cần xóa theo mã
            loai_ca loaica = CommonConstants.db.loai_ca.SingleOrDefault(n => n.id == id);
            ViewBag.id = loaica.id;
            if (loaica == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            CommonConstants.db.loai_ca.Remove(loaica);
            CommonConstants.db.SaveChanges();
            return RedirectToAction("QLloaica");
        }

        // Chỉnh sửa sản phẩm
        [HttpGet]
        public ActionResult SuaLoaiCa(int id)
        {
            // Lấy ra đối tượng sách cần xóa theo mã
            loai_ca loaica = CommonConstants.db.loai_ca.SingleOrDefault(n => n.id == id);
            ViewBag.id = loaica.id;
            if (loaica == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // Đưa dữ liệu vào DropdownList
            ViewBag.MaCa = new SelectList(CommonConstants.db.dong_ca.ToList().OrderBy(n => n.TenDongCa), "MaCa", "TenDongCa");
            return View(loaica);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaLoaiCa(loai_ca loaica, HttpPostedFileBase fileUpload, FormCollection f)
        {
            // Đưa dữ liệu vào DropdownList
            ViewBag.MaCa = new SelectList(CommonConstants.db.dong_ca.ToList().OrderBy(n => n.TenDongCa), "MaCa", "TenDongCa");
            // Kiểm tra đường dẫn file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa!";
                return View();
            }
            // Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    int id = Int32.Parse(f.Get("id"));
                    loaica = CommonConstants.db.loai_ca.Find(id);

                    if (fileUpload != null)
                    {
                        // Lưu tên file, lưu ý bổ sung thư viện System.IO
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        // Lưu đường dẫn của file
                        var path = Path.Combine(Server.MapPath("~/img"), fileName);
                        // Kiểm tra hình ảnh tồn tại chưa
                        if (System.IO.File.Exists(path))
                        {
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại!";
                        }
                        else
                        {
                            // Lưu hình ảnh vào đường dẫn
                            fileUpload.SaveAs(path);
                        }
                        loaica.Image = fileName;
                    }

                    loaica.fish_name = f.Get("fish_name");
                    loaica.Color = f.Get("Color");
                    loaica.Price = Decimal.Parse(f.Get("Price"));
                    loaica.Description = f.Get("Description");
                    loaica.NguonGoc = f.Get("NguonGoc");
                    loaica.SoLuong = Int32.Parse(f.Get("SoLuong"));
                    // Lưu vào CSDL
                    CommonConstants.db.SaveChanges();
                }
            }
            return RedirectToAction("QLloaica");
        }

        // QLctdonhang

        public ActionResult QLctdonhang(int ?page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            var ctdh = CommonConstants.db.CHITIETDONTHANGs.ToList();
            return View(ctdh.OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }

        // QLdondathang

        public ActionResult QLdondathang(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            var ddh = CommonConstants.db.DONDATHANGs.ToList();
            return View(ddh.OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }
    }
}