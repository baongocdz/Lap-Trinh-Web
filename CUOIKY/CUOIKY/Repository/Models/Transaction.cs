using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CUOIKY.Repository.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }

        public DateTime RequestDate { get; set; }

        public bool IsApproved { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public virtual User User { get; set; }
    }
}

