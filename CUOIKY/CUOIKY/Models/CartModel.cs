using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CUOIKY.Repository.Models;

namespace CUOIKY.Models
{
    public class CartModel
    {
        [Key]
        public int ProductID { get; set; } // Mã sách liên kết từ bảng SACH
        public int Quantity { get; set; } // Số lượng sách trong giỏ hàng

        // Điều hướng đến bảng SACH
        [ForeignKey("Product")]
        public virtual Product Product { get; set; }
    }
}
