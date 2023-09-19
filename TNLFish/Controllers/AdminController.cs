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

        // --------------------------- QUẢN LÝ DÒNG CÁ --------------------------- //

        public ActionResult QLdongca()
        {
            var dongca = from dong_ca in CommonConstants.db.dong_ca select dong_ca;
            return View(dongca);
        }

        // Thêm dongca
        [HttpGet]
        public ActionResult Themdongca()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Themdongca(dong_ca dongca, FormCollection f)
        {
            if (ModelState.IsValid)
            {
                var maca = f.Get("MaCa");
                var tendongca = f.Get("TenDongCa");
                if (String.IsNullOrEmpty(maca))
                {
                    ViewData["Loi1"] = "Phải nhập Mã cá!";
                }
                else if (String.IsNullOrEmpty(tendongca))
                {
                    ViewData["Loi2"] = "Phải nhập tên dòng cá!";
                }
                else
                {
                    dongca.MaCa = maca;
                    dongca.TenDongCa = tendongca;

                    // Lưu vào CSDL
                    CommonConstants.db.dong_ca.Add(dongca);
                    CommonConstants.db.SaveChanges();
                }
            }

            return RedirectToAction("QLdongca");
        }

        // Chỉnh sửa dongca
        [HttpGet]
        public ActionResult Suadongca(string maca)
        {
            // Lấy ra đối tượng dongca cần sửa theo mã
            dong_ca dongca = CommonConstants.db.dong_ca.SingleOrDefault(n => n.MaCa == maca);
            ViewBag.maca = dongca.MaCa;
            if (dongca == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dongca);
        }

        [HttpPost]
        public ActionResult Suadongca(FormCollection f)
        {
            if(ModelState.IsValid)
            {
                // Lấy ra đối tượng dongca cần sửa theo mã
                string maca = f.Get("MaCa");
                dong_ca dongca = CommonConstants.db.dong_ca.SingleOrDefault(n => n.MaCa == maca);
                // Set giá trị mới
                var tendongca = f.Get("TenDongCa");
                if (String.IsNullOrEmpty(tendongca))
                {
                    ViewData["Loi1"] = "Phải nhập tên dòng cá!";
                }
                else
                {
                    dongca.TenDongCa = tendongca;
                    // Lưu vào CSDL
                    CommonConstants.db.SaveChanges();
                }
            }

            return RedirectToAction("QLdongca");
        }

        // Hiển thị chi tiết dongca
        public ActionResult Chitietdongca(string maca)
        {
            // Lấy ra đối tượng dongca theo mã
            dong_ca dongca = CommonConstants.db.dong_ca.SingleOrDefault(n => n.MaCa == maca);
            ViewBag.maca = dongca.MaCa;
            if (dongca == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dongca);
        }

        // Xóa dongca
        [HttpGet]
        public ActionResult Xoadongca(string maca)
        {
            // Lấy ra đối tượng dongca cần xóa theo mã
            dong_ca dongca = CommonConstants.db.dong_ca.SingleOrDefault(n => n.MaCa == maca);
            ViewBag.maca = dongca.MaCa;
            if (dongca == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dongca);
        }

        [HttpPost, ActionName("Xoadongca")]
        public ActionResult Xacnhanxoa(string maca)
        {
            // Lấy ra đối tượng dongca cần xóa theo mã
            dong_ca dongca = CommonConstants.db.dong_ca.SingleOrDefault(n => n.MaCa == maca);
            ViewBag.maca = dongca.MaCa;
            if (dongca == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            CommonConstants.db.dong_ca.Remove(dongca);
            CommonConstants.db.SaveChanges();
            return RedirectToAction("QLdongca");
        }

        // --------------------------- QUẢN LÝ LOẠI CÁ --------------------------- //

        public ActionResult QLloaica(int ?page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            var loaica = CommonConstants.db.loai_ca.ToList();
            return View(loaica.OrderBy(n => n.id).ToPagedList(pageNumber, pageSize));
        }

        // Thêm loaica
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
        public ActionResult Themloaica(loai_ca loaica, HttpPostedFileBase fileUpload, FormCollection f)
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

        // Hiển thị chi tiết loaica
        public ActionResult ChitietLoaica(int id)
        {
            // Lấy ra đối tượng loaica theo mã
            loai_ca loaica = CommonConstants.db.loai_ca.SingleOrDefault(n => n.id == id);
            ViewBag.id = loaica.id;
            if(loaica == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loaica);
        }

        // Xóa loaica
        [HttpGet]
        public ActionResult XoaLoaica(int id)
        {
            // Lấy ra đối tượng loaica cần xóa theo mã
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
            // Lấy ra đối tượng loaica cần xóa theo mã
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

        // Chỉnh sửa loaica
        [HttpGet]
        public ActionResult SuaLoaiCa(int id)
        {
            // Lấy ra đối tượng loaica cần sửa theo mã
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
                    loaica.MaCa = f.Get("MaCa");
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

        // --------------------------- QUẢN LÝ CHI TIẾT ĐƠN HÀNG --------------------------- //

        public ActionResult QLctdonhang(int ?page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            var ctdh = CommonConstants.db.CHITIETDONTHANGs.ToList();
            return View(ctdh.OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }

        // --------------------------- QUẢN LÝ ĐƠN ĐẶT HÀNG --------------------------- //

        public ActionResult QLdondathang(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            var ddh = CommonConstants.db.DONDATHANGs.ToList();
            return View(ddh.OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }
    }
}