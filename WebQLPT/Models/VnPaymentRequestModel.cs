using System.ComponentModel.DataAnnotations;

namespace WebQLPT.Models
{
    public class VnPaymentRequestModel
    {
        [Required]
        public int HoaDonId { get; set; }

        public string? FullName { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
