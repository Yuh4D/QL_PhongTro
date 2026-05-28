using System.ComponentModel.DataAnnotations;

namespace WebQLPT.ViewModels
{
    public class CreateKhachThueViewModel
    {
        // Thông tin khách thuê
        [Required]
        [Display(Name = "Tên khách thuê")]
        public string? TenKhach { get; set; }
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        public string? CCCD { get; set; }

        [Display(Name = "Ngày thuê")]
        [DataType(DataType.Date)]
        public DateTime? NgayThue { get; set; }

        [Required]
        [Display(Name = "Phòng trọ")]
        public int PhongTroId { get; set; }

        // Tài khoản login
        [Required]
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}