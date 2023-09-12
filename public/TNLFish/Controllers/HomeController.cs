using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TNLFish.Models;
using System.Net.Mail;
using System.Net;
using TNLFish.common;

namespace TNLFish.Controllers
{
    public class HomeController : Controller
    {
        private List<loai_ca> Laycamoi(int count)
        {
            ShopCaCanhEntities db = CommonConstants.db;
            // Sắp xếp theo ngày cập nhật, sau đó lấy top @count
            return db.loai_ca.OrderByDescending(a => a.id).Take(count).ToList();
        }

        public ActionResult Index(int? page)
        {
            // tạo biến quy định trên mỗi trang
            int pageSize = 8;
            // tạo biến số trang
            int pageNum = (page ?? 1);

            var ds = Laycamoi(24);
            return View(ds.ToPagedList(pageNum, pageSize));
        }

        public ActionResult VendorFish()
        {
            var ds = from loai_ca in CommonConstants.db.loai_ca select loai_ca;
            return PartialView(ds);
        }

        public ActionResult Search(FormCollection collection)
        {
            var search = collection["search"].ToLower();
            var ds = from loai_ca in CommonConstants.db.loai_ca where loai_ca.fish_name.ToLower().Contains(search) select loai_ca;
            ViewBag.keyWord = collection["search"];
            return View(ds);
        }

        public ActionResult HdBetta()
        {
            return View();
        }

        public ActionResult HdGuppy()
        {
            return View();
        }

        public ActionResult HdKoi()
        {
            return View();
        }

        public ActionResult HdCaRong()
        {
            return View();
        }

        public ActionResult HdThuySinh()
        {
            return View();
        }

        public ActionResult HdLocCanh()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Contact(MailModel objMail)
        {
            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage(objMail.From, objMail.To);
                mail.Subject = objMail.Subject;
                mail.Body = objMail.Body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;

                NetworkCredential nc = new NetworkCredential("tnlfish20422@gmail.com", "TNLFish@170502"); // Enter seders User name and password       
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = nc;

                try
                {
                    smtp.Send(mail);

                    ViewBag.Message = "Mail được gửi thành công";
                    return View("Contact");
                }
                catch
                {
                    ViewBag.Message = "Lỗi hệ thống! Xin thử lại sau!";
                    return View();
                }
            }
            else
            {
                ViewBag.Message = "Lỗi hệ thống! Xin thử lại sau!";
                return View();
            }
        }
    }
}