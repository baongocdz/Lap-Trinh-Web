using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CUOIKY.Models
{
    public class EditProductModel
    {
        public int ProductId { get; set; }

        // Thông tin sản phẩm
        public int? ProductTypeId { get; set; }
        public int? Quantity { get; set; }
        public bool StatusSell { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Gmail { get; set; }

        // Thông tin mô tả
        [AllowHtml]
        public string? DescriptionText { get; set; }
        public int? Server { get; set; }
        public int? Planet { get; set; }
        public bool Portal { get; set; }
        public bool Disciple { get; set; }
        public decimal? Price { get; set; }

        // Hình ảnh
        public List<string>? ExistingImages { get; set; }

        [Display(Name = "Upload Images")]
        public List<IFormFile>? Images { get; set; }

        // Hình ảnh cần xóa
        public List<string>? ImagesToDelete { get; set; }
    }


}
