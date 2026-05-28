using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQLPT.Models
{
    public class DangTin
    {
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; }

        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }

        [Display(Name = "Giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        [Display(Name = "Hình ảnh")]
        public string HinhAnh { get; set; }

        [Display(Name = "Ngày đăng")]
        public DateTime NgayDang { get; set; } = DateTime.Now;

        [Display(Name = "Tên phòng")]
        public int PhongTroId { get; set; }

        [ValidateNever]
        [Display(Name = "Phòng trọ")]
        public PhongTro PhongTro { get; set; }

        [Display(Name = "Chủ trọ")]
        public int ChuTroId { get; set; }

        [ValidateNever]
        [Display(Name = "Chủ trọ")]
        public ChuTro ChuTro { get; set; }
    }
}
