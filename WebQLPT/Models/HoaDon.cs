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



        public int Thang { get; set; }

        public int Nam { get; set; }



        public DateTime NgayTao { get; set; }

        public DateTime HanThanhToan { get; set; }

     

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienPhong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienDien { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienNuoc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }


        public int ChiSoDienCu { get; set; }

        public int ChiSoDienMoi { get; set; }


        public int ChiSoNuocCu { get; set; }

        public int ChiSoNuocMoi { get; set; }

        public string? TrangThai { get; set; }

        public string? MaGiaoDichVNPay { get; set; }

        public DateTime? NgayThanhToan { get; set; }

        public string? PhuongThucThanhToan { get; set; }



    }
}