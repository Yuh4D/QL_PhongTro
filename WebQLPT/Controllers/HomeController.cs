using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebQLPT.Data;
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
            // THỐNG KÊ
            ViewBag.SoPhong = _context.PhongTros.Count();

            ViewBag.SoKhach = _context.KhachThues.Count();

            ViewBag.SoHoaDon = _context.HoaDons.Count();

            ViewBag.SoChuTro = _context.ChuTros.Count();

            // PHÒNG TRỐNG
            ViewBag.PhongTrong =
                _context.PhongTros
                    .Count(x => x.TrangThai == "Trống");

            // PHÒNG ĐÃ THUÊ
            ViewBag.PhongDaThue =
                _context.PhongTros
                    .Count(x => x.TrangThai == "Đã thuê");

            // HÓA ĐƠN CHƯA THANH TOÁN
            ViewBag.HoaDonChuaThanhToan =
                _context.HoaDons
                    .Count(x => x.TrangThai == "Chưa thanh toán");

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