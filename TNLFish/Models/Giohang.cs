using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TNLFish.Models
{
    public class Giohang
    {
        ShopCaCanhEntities db = new ShopCaCanhEntities();
        public int iID { get; set; }
        public string sfish_name { get; set; }
        public string sImage { get; set; }
        public Double dDongia { get; set; }
        public int iSoluong { get; set; }

        public Double dThanhtien
        {
            get { return dDongia * iSoluong; }
        }

        //Khởi tạo giỏ hàng với mã sách được truyền vào với Soluong mặc định là 1
        public Giohang(int id)
        {
            this.iID = id;
            loai_ca fish = db.loai_ca.Single(n => n.id == iID);
            sfish_name = fish.fish_name;
            sImage = fish.Image;
            dDongia = double.Parse(fish.Price.ToString());
            iSoluong = 1;
        }
    }
}