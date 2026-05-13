using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQLPT.Models
{
    public class HoaDon
    {
        public int Id { get; set; }

        public int PhongTroId { get; set; }

        [ValidateNever]
        public PhongTro? PhongTro { get; set; }

        public int KhachThueId { get; set; }

        [ValidateNever]
        public KhachThue? KhachThue { get; set; }

        public DateTime NgayTao { get; set; }
        public DateTime HanThanhToan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        public string? TrangThai { get; set; } // MoiTao, DaGui, DaThanhToan

        [ValidateNever]
        public List<HoaDonChiTiet>? HoaDonChiTiets { get; set; }

        [NotMapped]
        public List<HoaDonChiTiet> ChiTiets { get; set; } = new();
    }
}
