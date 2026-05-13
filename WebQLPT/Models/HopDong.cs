using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQLPT.Models
{
    public class HopDong
    {
        public int Id { get; set; }

        [Display(Name = "Phòng")]
        public int PhongTroId { get; set; }

        [Display(Name = "Khách thuê")]
        public int KhachThueId { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; }

        [Display(Name = "Ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; }

        [Display(Name = "Tiền cọc")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TienCoc { get; set; }

        public string? NoiDung { get; set; }

        [ValidateNever]
        public PhongTro? PhongTro { get; set; }
        [ValidateNever]
        public KhachThue? KhachThue { get; set; }
    
}
}
