using System.ComponentModel.DataAnnotations;

namespace CUOIKY.Models
{
    public class EditUserModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền cho người dùng.")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        public string Password { get; set; }

        public string UserNameString { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Số dư phải là số dương.")]
        public decimal? Balance { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Gmail { get; set; }
    }
}
