using System.ComponentModel.DataAnnotations;

namespace WebQLPT.ViewModels
{
    public class CreateKhachThueViewModel
    {
        // Thông tin khách thuê
        [Required]
        public string? TenKhach { get; set; }

        public string? SoDienThoai { get; set; }

        public string? CCCD { get; set; }

        [Display(Name = "Ngày thuê")]
        [DataType(DataType.Date)]
        public DateTime? NgayThue { get; set; }

        [Required]
        public int PhongTroId { get; set; }

        // Tài khoản login
        [Required]
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}