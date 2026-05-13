using WebQLPT.Models;

namespace WebQLPT.Services
{
    public class HopDongService
    {
        public string GenerateHopDong(HopDong hopDong, PhongTro phong, KhachThue khach, ChuTro chuTro)
        {
            return $@"
<h2 style='text-align:center'>HỢP ĐỒNG THUÊ PHÒNG</h2>

<p><strong>Bên cho thuê:</strong> {chuTro.TenChuTro} - {chuTro.SoDienThoai}</p>
<p><strong>Bên thuê:</strong> {khach.TenKhach} - {khach.SoDienThoai}</p>

<p><strong>Phòng:</strong> {phong.TenPhong}</p>
<p><strong>Giá:</strong> {phong.Gia:N0} VNĐ</p>

<p><strong>Thời gian:</strong> {hopDong.NgayBatDau:dd/MM/yyyy} - {hopDong.NgayKetThuc:dd/MM/yyyy}</p>

<p><strong>Tiền cọc:</strong> {hopDong.TienCoc:N0} VNĐ</p>

<p><strong>Điều khoản:</strong></p>
<ul>
<li>Thanh toán đúng hạn</li>
<li>Giữ gìn tài sản</li>
<li>Không gây mất trật tự</li>
</ul>

<br/>
<div style='display:flex; justify-content:space-between'>
    <div>Chủ trọ<br/><br/>(Ký)</div>
    <div>Khách thuê<br/><br/>(Ký)</div>
</div>
";
        }
    }
}