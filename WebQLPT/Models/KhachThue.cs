using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebQLPT.Models
{
    public class KhachThue
    {
        public int Id { get; set; }

        [Display(Name = "Tên khách")]
        public string? TenKhach { get; set; }

        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        public string? CCCD { get; set; }

        [Display(Name = "Ngày thuê")]
        public DateTime? NgayThue { get; set; }

        public DateTime? NgayKetThuc { get; set; }
        public bool DangO { get; set; } = true;

        [Display(Name = "ID Phòng")]
        public int PhongTroId { get; set; }

        [ValidateNever]
        public PhongTro? PhongTro { get; set; }

        [ValidateNever]
        public User? User { get; set; }
    }
}
