using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TNLFish.Models
{
    public class LoaiCa
    {
        public int Id { get; set; }
        public string DongCa { get; set; }
        public string FishName { get; set; }
        public string UrlImage { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string NguonGoc { get; set; }
        public decimal Price { get; set; }
        public int SoLuong { get; set; }
    }
}