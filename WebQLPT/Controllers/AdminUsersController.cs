using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQLPT.Data;

namespace WebQLPT.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminUsersController : Controller
    {
        private readonly AppDbContext _context;

        public AdminUsersController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // DANH SÁCH TÀI KHOẢN
        // =========================
        public async Task<IActionResult> Index(string keyword)
        {
            var query = _context.Users
                .Include(x => x.ChuTro)
                .Include(x => x.KhachThue)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    (x.Username ?? "").Contains(keyword) ||
                    (x.Role ?? "").Contains(keyword) ||

                    (x.ChuTro != null && 
                     (x.ChuTro.TenChuTro ?? "").Contains(keyword)) ||

                    (x.KhachThue != null && 
                     (x.KhachThue.TenKhach ?? "").Contains(keyword))
                );
            }

            // SORT
            var users = await query
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(users);
        }

        // =========================
        // DUYỆT TÀI KHOẢN
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                TempData["Error"] =
                    "Không tìm thấy tài khoản";

                return RedirectToAction(nameof(Index));
            }

            // Đã duyệt rồi
            if (user.IsApproved)
            {
                TempData["Error"] =
                    "Tài khoản đã được duyệt trước đó";

                return RedirectToAction(nameof(Index));
            }

            user.IsApproved = true;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Đã duyệt tài khoản '{user.Username}'";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // KHÓA TÀI KHOẢN
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                TempData["Error"] =
                    "Không tìm thấy tài khoản";

                return RedirectToAction(nameof(Index));
            }

            // Không khóa admin
            if (user.Role == "admin")
            {
                TempData["Error"] =
                    "Không thể khóa tài khoản admin";

                return RedirectToAction(nameof(Index));
            }

            // Đã khóa rồi
            if (!user.IsApproved)
            {
                TempData["Error"] =
                    "Tài khoản đã bị khóa trước đó";

                return RedirectToAction(nameof(Index));
            }

            user.IsApproved = false;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Đã khóa tài khoản '{user.Username}'";

            return RedirectToAction(nameof(Index));
        }
    }
}