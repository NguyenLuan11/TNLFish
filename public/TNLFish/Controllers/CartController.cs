using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNLFish.Models;
using TNLFish.common;

namespace TNLFish.Controllers
{
    public class CartController : Controller
    {
        // Lấy giỏ hàng
        public List<Giohang> LayGiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if(lstGiohang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì khởi tạo listGiohang
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }

        // Thêm giỏ hàng
        public ActionResult ThemGiohang(int ID, string strURL)
        {
            // Lấy ra Session Giohang
            List<Giohang> lstGiohang = LayGiohang();
            // Kiểm tra Loại cá này tồn tại trong Session["Giohang"] chưa?
            Giohang sanpham = lstGiohang.Find(n => n.iID == ID);
            if (sanpham == null)
            {
                sanpham = new Giohang(ID);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }

        // Tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }

        // Tổng tiền
        private double TongTien()
        {
            double dTongTien = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                dTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return dTongTien;
        }

        // Xây dựng trang giỏ hàng
        public ActionResult Giohang()
        {
            List<Giohang> lstGiohang = LayGiohang();
            if(lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGiohang);
        }

        // Tạo Partial view để hiển thị thông tin giỏ hàng
        public ActionResult GiohangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        // Xóa Giohang
        public ActionResult XoaGiohang(int iMasp)
        {
            // lấy giỏ hàng từ Session
            List<Giohang> lstGiohang = LayGiohang();
            // Kiểm tra sách đã có trong Session["Giohang"]
            Giohang sanpham = lstGiohang.FirstOrDefault(n => n.iID == iMasp);
            // Nếu tồn tại thì cho sửa số lượng
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iID == iMasp);
                return RedirectToAction("Giohang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Giohang");
        }

        // Cập nhật Giohang
        public ActionResult CapnhatGiohang(int iMasp, FormCollection f)
        {
            // lấy giỏ hàng từ Session
            List<Giohang> lstGiohang = LayGiohang();
            // Kiểm tra sách đã có trong Session["Giohang"]
            Giohang sanpham = lstGiohang.FirstOrDefault(n => n.iID == iMasp);
            // Nếu tồn tại thì cho sửa số lượng
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }

        // Xóa tất cả giỏ hàng
        public ActionResult XoatatcaGiohang()
        {
            // lấy giỏ hàng từ Session
            List<Giohang> lstGiohang = LayGiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Hiển thị view DatHang để cập nhật các thông tin cho đơn hàng
        [HttpGet]
        public ActionResult DatHang()
        {
            // Kiểm tra đăng nhập
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Lấy giỏ hàng từ Session
            List<Giohang> lstGiohang = LayGiohang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGiohang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            // Thêm đơn hàng
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Giohang> lstGiohang = LayGiohang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = string.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            CommonConstants.db.DONDATHANGs.Add(ddh);
            CommonConstants.db.SaveChanges();
            // Thêm chi tiết đơn hàng
            foreach (var item in lstGiohang)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.id = item.iID;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (Decimal)item.dDongia;
                CommonConstants.db.CHITIETDONTHANGs.Add(ctdh);

                // Cập nhật lại số lượng loại cá sau khi bán
                int id = item.iID;
                loai_ca loaica = CommonConstants.db.loai_ca.Find(id);
                loaica.SoLuong -= item.iSoluong;
            }
            CommonConstants.db.SaveChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "cart");
        }

        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}