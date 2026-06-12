using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebQLPT.Data;
using WebQLPT.Helpers;
using WebQLPT.Models.ViewModels;

namespace WebQLPT.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // ADMIN
            if (User.IsInRole("admin"))
            {
                ViewBag.SoPhong = _context.PhongTros.Count();

                ViewBag.SoKhach = _context.KhachThues.Count();

                ViewBag.SoChuTro = _context.ChuTros.Count();

                ViewBag.SoHoaDon =
                    _context.HoaDons.Count();

                ViewBag.SoHopDong =
                    _context.HopDongs.Count();

                return View();
            }

            // CHỦ TRỌ
            if (User.IsInRole("chutro"))
            {
                var chuTroId =
                    UserHelper.GetChuTroId(User);

                ViewBag.SoPhong =
                    _context.PhongTros
                        .Count(p => p.ChuTroId == chuTroId);

                ViewBag.PhongDaThue =
                    _context.PhongTros
                        .Count(p =>
                            p.ChuTroId == chuTroId &&
                            p.TrangThai == "Đã thuê");

                ViewBag.SoKhach =
                    _context.KhachThues
                        .Count(k =>
                            k.PhongTro != null &&
                            k.PhongTro.ChuTroId == chuTroId);

                ViewBag.HoaDonChuaThanhToan =
                    _context.HoaDons
                        .Count(h =>
                            h.PhongTro != null &&
                            h.PhongTro.ChuTroId == chuTroId &&
                            h.TrangThai != "Đã thanh toán");

                ViewBag.DoanhThu =
                    _context.HoaDons
                        .Where(h =>
                            h.PhongTro != null &&
                            h.PhongTro.ChuTroId == chuTroId &&
                            h.TrangThai == "Đã thanh toán")
                        .Sum(h => (decimal?)h.TongTien) ?? 0;

                return View();
            }

            // KHÁCH THUÊ
            if (User.IsInRole("khachthue"))
            {
                var khachId = UserHelper.GetKhachThueId(User);

                if (khachId == null)
                {
                    return Forbid();
                }

                var hopDong = _context.HopDongs
                    .Include(h => h.PhongTro)
                    .Where(h => h.KhachThueId == khachId.Value)
                    .OrderByDescending(h => h.NgayBatDau)
                    .FirstOrDefault();

                ViewBag.TenPhong =
                    hopDong?.PhongTro?.TenPhong ?? "Chưa có phòng";

                ViewBag.GiaPhong =
                    hopDong?.PhongTro?.Gia ?? 0;

                ViewBag.HanHopDong =
                    hopDong?.NgayKetThuc;

                ViewBag.HoaDonChuaThanhToan =
                    _context.HoaDons.Count(h =>
                        h.KhachThueId == khachId.Value &&
                        h.TrangThai != "Đã thanh toán");

                return View();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
            });
        }
    }
}
