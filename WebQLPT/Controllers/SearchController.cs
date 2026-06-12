using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQLPT.Data;

namespace WebQLPT.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ViewBag.Keyword = "";
                ViewBag.PhongTro = new List<Models.PhongTro>();
                ViewBag.KhachThue = new List<Models.KhachThue>();
                ViewBag.ChuTro = new List<Models.ChuTro>();

                return View();
            }

            keyword = keyword.Trim();

            //Phòng trọ
            var phongTro = await _context.PhongTros
                .Where(p =>
                    (p.TenPhong ?? "").Contains(keyword) ||
                    (p.MoTa ?? "").Contains(keyword) ||
                    p.Gia.ToString().Contains(keyword))
                .ToListAsync();

            //Khách thuê
            var khachThue = await _context.KhachThues
                .Include(k => k.PhongTro)
                .Where(k =>
                    (k.TenKhach ?? "").Contains(keyword) ||
                    (k.SoDienThoai ?? "").Contains(keyword) ||
                    (k.CCCD ?? "").Contains(keyword))
                .ToListAsync();

            //Chủ trọ
            var chuTro = await _context.ChuTros
                .Where(c =>
                    (c.TenChuTro ?? "").Contains(keyword) ||
                    (c.SoDienThoai ?? "").Contains(keyword) ||
                    (c.Email ?? "").Contains(keyword) ||
                    (c.DiaChi ?? "").Contains(keyword))
                .ToListAsync();

            ViewBag.Keyword = keyword;
            ViewBag.PhongTro = phongTro;
            ViewBag.KhachThue = khachThue;
            ViewBag.ChuTro = chuTro;

            return View();
        }
    }
}
