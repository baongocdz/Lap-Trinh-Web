//using CUOIKY.Repository.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CUOIKY.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public int? RoleId { get; set; }

        public string? UserNameString { get; set; }

        public decimal? Balance { get; set; }

        [StringLength(255)]
        public string? Gmail { get; set; }
    }
}
