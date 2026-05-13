using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebQLPT.Data;
using WebQLPT.Models;
using WebQLPT.Models.ViewModels;
using WebQLPT.ViewModels;

namespace WebQLPT.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(AppDbContext context)
        {
            _context = context;

            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users
                .FirstOrDefault(x => x.Username == model.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu");
                return View(model);
            }

            // Kiểm tra duyệt
            if (!user.IsApproved)
            {
                ModelState.AddModelError("", "Tài khoản chưa được admin duyệt");
                return View(model);
            }

            // Verify password
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu");
                return View(model);
            }

            // Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),

                new Claim(ClaimTypes.Role, user.Role),

                new Claim("UserId", user.Id.ToString())
            };

            if (user.ChuTroId.HasValue)
            {
                claims.Add(new Claim(
                    "ChuTroId",
                    user.ChuTroId.Value.ToString()));
            }

            if (user.KhachThueId.HasValue)
            {
                claims.Add(new Claim(
                    "KhachThueId",
                    user.KhachThueId.Value.ToString()));
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            if (user.Role == "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.Role == "chutro")
            {
                return RedirectToAction("Index", "PhongTroes");
            }

            if (user.Role == "khachthue")
            {
                return RedirectToAction("Index", "DangTins");
            }

            return RedirectToAction("Index", "Home");
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }

        // AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra username tồn tại
            var checkUser = _context.Users
                .FirstOrDefault(x => x.Username == model.Username);

            if (checkUser != null)
            {
                ModelState.AddModelError("", "Tài khoản đã tồn tại");
                return View(model);
            }

            // Tạo chủ trọ
            var chuTro = new ChuTro
            {
                TenChuTro = model.TenChuTro,
                SoDienThoai = model.SoDienThoai,
                Email = model.Email,
                DiaChi = model.DiaChi
            };

            _context.ChuTros.Add(chuTro);

            await _context.SaveChangesAsync();

            // Tạo user
            var user = new User
            {
                Username = model.Username,

                Role = "chutro",

                IsApproved = false,

                ChuTroId = chuTro.Id
            };

            // Hash password
            user.PasswordHash = _passwordHasher
                .HashPassword(user, model.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Đăng ký thành công. Vui lòng chờ admin duyệt tài khoản.";

            return RedirectToAction(nameof(Login));
        }
        public IActionResult GenerateAdmin()
        {
            var user = new User();

            var hash = _passwordHasher
                .HashPassword(user, "123456");

            return Content(hash);
        }

    }
}