using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebQLPT.Models
{
    public class HoaDonChiTiet
    {
        public int Id { get; set; }

        public int HoaDonId { get; set; }

        [ValidateNever]
        public HoaDon HoaDon { get; set; }

        public string NoiDung { get; set; }

        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal HeSo { get; set; }

        public decimal ThanhTien { get; set; }
    }
}
