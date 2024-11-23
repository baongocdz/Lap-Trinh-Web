using System.ComponentModel.DataAnnotations;

namespace CUOIKY.Models
{
    public class AddProductModel
    {
        [Required]
        public int ProductTypeId { get; set; } // 1 for Account, 2 for Item

        [Required]
        public int Quantity { get; set; }

        [Required]
        public bool StatusSell { get; set; }

        // Account Details
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Gmail { get; set; }

        // Description fields
        [Required]
        public string DescriptionText { get; set; }

        public int? Server { get; set; }
        public int? Planet { get; set; }
        public bool Portal { get; set; }
        public bool Disciple { get; set; }
        public decimal? Price { get; set; }

        // Image files
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
