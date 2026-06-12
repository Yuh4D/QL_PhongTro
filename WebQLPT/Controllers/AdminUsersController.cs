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


        public async Task<IActionResult> Index(string keyword)
        {
            var query = _context.Users
                .Include(x => x.ChuTro)
                .Include(x => x.KhachThue)
                .AsQueryable();

            var users = await query
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                users = users.Where(x =>
                    (x.Username ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (x.Role ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (x.ChuTro != null && (x.ChuTro.TenChuTro ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (x.KhachThue != null && (x.KhachThue.TenKhach ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (x.IsApproved ? "đã duyệt" : "chưa duyệt").Contains(keyword, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return View(users);
        }


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

          
            if (user.Role == "admin")
            {
                TempData["Error"] =
                    "Không thể khóa tài khoản admin";

                return RedirectToAction(nameof(Index));
            }

            
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