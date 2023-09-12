using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Aspose.Cells;

using TNLFish.Models;
using TNLFish.common;

namespace TNLFish.Controllers
{
    public class ExportDataController : Controller
    {
        // GET: ExportData
        public ActionResult Index()
        {
            return View();
        }

        private List<LoaiCa> LoaiCaItems()
        {
            var resultList = new List<LoaiCa>();
            for (int i = 0; i < CommonConstants.db.loai_ca.Count(); i++)
            {

                var dataLoaiCa = CommonConstants.db.loai_ca.SingleOrDefault(x => x.id == i);
                if(dataLoaiCa != null)
                {
                    var loaica = new LoaiCa()
                    {
                        Id = dataLoaiCa.id,
                        DongCa = dataLoaiCa.dong_ca.TenDongCa,
                        FishName = dataLoaiCa.fish_name,
                        UrlImage = dataLoaiCa.Image,
                        Color = dataLoaiCa.Color,
                        Description = dataLoaiCa.Description,
                        NguonGoc = dataLoaiCa.NguonGoc,
                        Price = (Decimal)dataLoaiCa.Price,
                        SoLuong = (Int32)dataLoaiCa.SoLuong
                    };
                    resultList.Add(loaica);
                }
            }
            return resultList;
        }

        private Stream CreateExcelFile(Stream stream = null)
        {
            var list = LoaiCaItems();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                // Tạo author cho file Excel
                excelPackage.Workbook.Properties.Author = "Hanker";
                // Tạo title cho file Excel
                excelPackage.Workbook.Properties.Title = "Infomation of Fish";
                // thêm tí comments vào làm màu 
                excelPackage.Workbook.Properties.Comments = "This is list kinds of fish!";
                // Add Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("First Sheet");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var workSheet = excelPackage.Workbook.Worksheets["First Sheet"];
                // Đổ data vào Excel file
                workSheet.Cells[1, 1].LoadFromCollection(list, true, TableStyles.Dark9);
                // Format for Excel file
                BindingFormatForExcel(workSheet, list);
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        private void BindingFormatForExcel(ExcelWorksheet worksheet, List<LoaiCa> listItems)
        {
            // Set default width cho tất cả column
            worksheet.DefaultColWidth = 10;
            // Tự động xuống hàng khi text quá dài
            worksheet.Cells.Style.WrapText = true;
            // Tạo header
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Dòng cá";
            worksheet.Cells[1, 3].Value = "Tên cá";
            worksheet.Cells[1, 4].Value = "Hình ảnh";
            worksheet.Cells[1, 5].Value = "Màu sắc";
            worksheet.Cells[1, 6].Value = "Mô tả";
            worksheet.Cells[1, 7].Value = "Nguồn gốc";
            worksheet.Cells[1, 8].Value = "Giá bán";
            worksheet.Cells[1, 9].Value = "Số lượng";

            // Lấy range vào tạo format cho range đó ở đây là từ B1 tới D1
            using (var range = worksheet.Cells["B1:D1"])
            {
                // Set PatternType
                range.Style.Fill.PatternType = ExcelFillStyle.DarkGray;
                // Set Màu cho Background
                range.Style.Fill.BackgroundColor.SetColor(Color.DeepSkyBlue);
                // Canh giữa cho các text
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                // Set Font cho text  trong Range hiện tại
                //range.Style.Font.SetFromFont(new Font("Arial", 10));
                // Set Border
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                // Set màu ch Border
                range.Style.Border.Bottom.Color.SetColor(Color.DarkBlue);
            }

            /*
            //Initialize license
            Aspose.Cells.License cellsLicense = new Aspose.Cells.License();
            cellsLicense.SetLicense("Aspose.Total.lic");
            // Instantiating a Workbook object
            Workbook workbook = new Workbook();
            // Set the reference of a worksheet using sheet index
            Worksheet cellworksheet = workbook.Worksheets[0];
            // Adding a picture at row and column cell
            */

            // Đỗ dữ liệu từ list vào 
            for (int i = 0; i < listItems.Count; i++)
            {
                var item = listItems[i];
                worksheet.Cells[i + 2, 1].Value = item.Id + 1;
                worksheet.Cells[i + 2, 2].Value = item.DongCa;
                worksheet.Cells[i + 2, 3].Value = item.FishName;
                worksheet.Cells[i + 2, 4].Value = item.UrlImage;
                //worksheet.Cells[i + 2, 4].Value = cellworksheet.Pictures.Add(2, 4, item.UrlImage);
                worksheet.Cells[i + 2, 5].Value = item.Color; 
                worksheet.Cells[i + 2, 6].Value = item.Description;
                worksheet.Cells[i + 2, 7].Value = item.NguonGoc;
                worksheet.Cells[i + 2, 8].Value = item.Price;
                worksheet.Cells[i + 2, 9].Value = item.SoLuong;
            }
            // Format lại định dạng xuất ra ở cột Price
            worksheet.Cells[2, 7, listItems.Count + 4, 8].Style.Numberformat.Format = "#,##0 \"VNĐ\"";
            // fix lại width của column với minimum width là 15
            worksheet.Cells[1, 1, listItems.Count + 5, 4].AutoFitColumns(24);
        }

        [HttpGet]
        public ActionResult Export()
        {
            // Gọi lại hàm để tạo file excel
            var stream = CreateExcelFile();
            // Tạo buffer memory strean để hứng file excel
            var buffer = stream as MemoryStream;
            // Đây là content Type dành cho file excel, còn rất nhiều content-type khác nhưng cái này mình thấy okay nhất
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            // Dòng này rất quan trọng, vì chạy trên firefox hay IE thì dòng này sẽ hiện Save As dialog cho người dùng chọn thư mục để lưu
            // File name của Excel này là ExcelDemo
            Response.AddHeader("Content-Disposition", "attachment; filename=LoaiCa.xlsx");
            // Lưu file excel của chúng ta như 1 mảng byte để trả về response
            Response.BinaryWrite(buffer.ToArray());
            // Send tất cả ouput bytes về phía clients
            Response.Flush();
            Response.End();
            // Redirect về luôn trang index <img draggable="false" role="img" class="emoji" alt="😀" src="https://s0.wp.com/wp-content/mu-plugins/wpcom-smileys/twemoji/2/svg/1f600.svg" scale="0">
            return RedirectToAction("Index", "Admin");
        }

        /*
        private DataTable ReadFromExcelfile(string path, string sheetName)
        {
            // Khởi tạo data table
            DataTable dt = new DataTable();
            // Load file excel và các setting ban đầu
            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                if (package.Workbook.Worksheets.Count < 1)
                {
                    // Log - Không có sheet nào tồn tại trong file excel của bạn
                    return null;
                }
                // Khởi Lấy Sheet đầu tiện trong file Excel để truy vấn, truyền vào name của Sheet để lấy ra sheet cần, nếu name = null thì lấy sheet đầu tiên
                ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == sheetName) ?? package.Workbook.Worksheets.FirstOrDefault();
                // Đọc tất cả các header
                foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                {
                    dt.Columns.Add(firstRowCell.Text);
                }
                // Đọc tất cả data bắt đầu từ row thứ 2
                for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                {
                    // Lấy 1 row trong excel để truy vấn
                    var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                    // tạo 1 row trong data table
                    var newRow = dt.NewRow();
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;

                    }
                    dt.Rows.Add(newRow);
                }
            }
            return dt;
        }

        [HttpGet]
        public ActionResult ReadFromExcel()
        {
            var data = ReadFromExcelfile(@"D:\LoaiCa.xlsx", "First Sheet");
            return View(data);
        }
        */
    }
}