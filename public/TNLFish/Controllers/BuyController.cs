using System;
using System.Linq;
using System.Web.Mvc;
using TNLFish.Models;
using PagedList;
using TNLFish.common;

namespace TNLFish.Controllers
{
    public class BuyController : Controller
    {
        // GET: Buy
        public ActionResult Index(int? page)
        {
            // tạo biến quy định trên mỗi trang
            int pageSize = 6;
            // tạo biến số trang
            int pageNum = (page ?? 1);
            var ds = CommonConstants.db.loai_ca.Take(80).ToList(); ;
            return View(ds.ToPagedList(pageNum, pageSize));
        }

        [HttpGet]
        public ActionResult Detail(int id)
        {
            loai_ca fish = CommonConstants.db.loai_ca.Find(id);
            return View(fish);
        }

        [HttpPost]
        public ActionResult Detail(FormCollection f)
        {
            int id = Int32.Parse(f.Get("id"));
            loai_ca fish = CommonConstants.db.loai_ca.Find(id);
            fish.fish_name = f.Get("fish_name");
            fish.Image = f.Get("Image");
            fish.Color = f.Get("Color");
            fish.Description = f.Get("Description");
            fish.NguonGoc = f.Get("NguonGoc");
            fish.Price = Decimal.Parse(f.Get("Price"));
            fish.SoLuong = Int32.Parse(f.Get("SoLuong"));
            CommonConstants.db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Dongca()
        {
            var dongca = from dong_ca in CommonConstants.db.dong_ca select dong_ca;
            return PartialView(dongca);
        }

        public ActionResult SPTheoDongca(string maca)
        {
            var fish = from loai_ca in CommonConstants.db.loai_ca where loai_ca.MaCa == maca select loai_ca;
            return PartialView(fish);
        }

        public ActionResult SearchPartial()
        {
            return PartialView();
        }

        public ActionResult SearchBuy(FormCollection collection)
        {
            var search = collection["keyWord"].ToLower();
            var ds = from loai_ca in CommonConstants.db.loai_ca where loai_ca.fish_name.ToLower().Contains(search) select loai_ca;
            ViewBag.keyWordSearch = collection["keyWord"];
            return View(ds);
        }
    }
}