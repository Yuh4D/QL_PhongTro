using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        // Danh sách user
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(x => x.ChuTro)
                .Include(x => x.KhachThue)
                .ToListAsync();

            return View(users);
        }

        // Duyệt tài khoản
        public async Task<IActionResult> Approve(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsApproved = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Khóa tài khoản
        public async Task<IActionResult> Lock(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Không khóa admin
            if (user.Role == "admin")
            {
                TempData["Error"] =
                    "Không thể khóa admin";

                return RedirectToAction(nameof(Index));
            }

            user.IsApproved = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

