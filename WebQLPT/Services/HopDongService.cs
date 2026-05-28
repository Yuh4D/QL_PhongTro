using WebQLPT.Models;


namespace WebQLPT.Services
{
    public class HopDongService
    {
        public string GenerateHopDong(
            HopDong hopDong,
            PhongTro phong,
            KhachThue khach,
            ChuTro chuTro)
        {
            var dieuKhoanThem =
                string.IsNullOrWhiteSpace(hopDong.DieuKhoanThem)
                ? ""
                : $"<li>{hopDong.DieuKhoanThem}</li>";

            return $@"
<div style='font-family:Arial; line-height:1.8'>

    <h2 style='text-align:center; color:#0d6efd'>
        HỢP ĐỒNG THUÊ PHÒNG
    </h2>

    <hr/>

    <p>
        <strong>Bên cho thuê:</strong>
        {chuTro.TenChuTro}
        - {chuTro.SoDienThoai}
    </p>

    <p>
        <strong>Bên thuê:</strong>
        {khach.TenKhach}
        - {khach.SoDienThoai}
    </p>

    <p>
        <strong>Phòng:</strong>
        {phong.TenPhong}
    </p>

    <p>
        <strong>Giá phòng:</strong>
        {phong.Gia:N0} VNĐ
    </p>

    <p>
        <strong>Thời gian thuê:</strong>
        {hopDong.NgayBatDau:dd/MM/yyyy}
        -
        {hopDong.NgayKetThuc:dd/MM/yyyy}
    </p>

    <p>
        <strong>Tiền cọc:</strong>
        {hopDong.TienCoc:N0} VNĐ
    </p>

    <h4>Điều khoản</h4>

    <ul>
        <li>Thanh toán đúng hạn</li>
        <li>Giữ gìn tài sản</li>
        <li>Không gây mất trật tự</li>
        {dieuKhoanThem}
    </ul>

    <br/>

    <div style='display:flex;
                justify-content:space-between;
                margin-top:60px'>

        <div style='text-align:center'>
            Chủ trọ
            <br/><br/><br/>
            (Ký tên)
        </div>

        <div style='text-align:center'>
            Khách thuê
            <br/><br/><br/>
            (Ký tên)
        </div>

    </div>

</div>";
        }
    }
}