using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebQLPT.Data;
using WebQLPT.Models;
using WebQLPT.Helpers;

namespace WebQLPT.Controllers
{
    [Authorize(Roles = "admin,chutro")]
    public class KhachThuesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public KhachThuesController(AppDbContext context)
        {
            _context = context;

            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: KhachThues
        public async Task<IActionResult> Index(string keyword)
        {
            var query = _context.KhachThues.Include(k => k.PhongTro).AsQueryable();

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                query = query.Where(k =>
                    k.PhongTro.ChuTroId == chuTroId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(k => k.TenKhach.Contains(keyword));
            }

            return View(await query.ToListAsync());
        }

        // GET: KhachThues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachThue = await _context.KhachThues
                .Include(k => k.PhongTro)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (khachThue == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                var phong = await _context.PhongTros
                    .FirstOrDefaultAsync(p =>
                        p.Id == khachThue.PhongTroId);

                if (phong == null ||
                    phong.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            return View(khachThue);
        }

        // GET: KhachThues/Create
        public IActionResult Create()
        {
            var phongTros = _context.PhongTros.AsQueryable();

        
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                phongTros = phongTros
                    .Where(p => p.ChuTroId == chuTroId);
            }

            ViewData["PhongTroId"] = new SelectList(phongTros, "Id", "TenPhong");
            return View();
        }

        // POST: KhachThues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenKhach,SoDienThoai,CCCD,NgayThue,PhongTroId")] KhachThue khachThue)
        {
            if (!ModelState.IsValid)
            {
                return View(khachThue);
            }

            // Chủ trọ chỉ được tạo khách cho phòng mình
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                var phong = await _context.PhongTros
                    .FirstOrDefaultAsync(p =>
                        p.Id == khachThue.PhongTroId);

                if (phong == null ||
                    phong.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            // Không được thuê phòng đã có khách
            var daCoKhach = await _context.KhachThues
                .AnyAsync(k => k.PhongTroId == khachThue.PhongTroId);

            if (daCoKhach)
            {
                TempData["Error"] =
                    "Phòng này đã có người thuê!";

                return RedirectToAction(nameof(Create));
            }

            // Tạo khách thuê
            _context.KhachThues.Add(khachThue);

            await _context.SaveChangesAsync();

            // AUTO TẠO USER ACCOUNT
            var username =
                khachThue.SoDienThoai;

            var password =
                "123456";

            var user = new User
            {
                Username = username,

                Role = "khachthue",

                IsApproved = true,

                KhachThueId = khachThue.Id
            };

            user.PasswordHash = _passwordHasher
                .HashPassword(user, password);

            _context.Users.Add(user);

            // Update trạng thái phòng
            var phongTro = await _context.PhongTros
                .FindAsync(khachThue.PhongTroId);

            if (phongTro != null)
            {
                phongTro.TrangThai = "Đã thuê";
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Đã tạo tài khoản khách thuê. " +
                $"Username: {username} - Password: 123456";

            return RedirectToAction(nameof(Index));
        }

        // GET: KhachThues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachThue = await _context.KhachThues
                .Include(k => k.PhongTro)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (khachThue == null)
            {
                return NotFound();
            }

            // Chủ trọ chỉ sửa khách của mình
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (khachThue.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            // Dropdown phòng
            var phongTros = _context.PhongTros.AsQueryable();

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                phongTros = phongTros
                    .Where(p => p.ChuTroId == chuTroId);
            }

            ViewData["PhongTroId"] = new SelectList(
                phongTros,
                "Id",
                "TenPhong",
                khachThue.PhongTroId);

            return View(khachThue);
        }

        // POST: KhachThues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    [Bind("Id,TenKhach,SoDienThoai,CCCD,NgayThue,PhongTroId")]
    KhachThue khachThue)
        {
            if (id != khachThue.Id)
            {
                return NotFound();
            }

            // Lấy dữ liệu gốc
            var oldKhach = await _context.KhachThues
                .Include(k => k.PhongTro)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (oldKhach == null)
            {
                return NotFound();
            }

            // Chủ trọ chỉ sửa khách của mình
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (oldKhach.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    oldKhach.TenKhach = khachThue.TenKhach;
                    oldKhach.SoDienThoai = khachThue.SoDienThoai;
                    oldKhach.CCCD = khachThue.CCCD;
                    oldKhach.NgayThue = khachThue.NgayThue;
                    oldKhach.PhongTroId = khachThue.PhongTroId;

                    _context.Update(oldKhach);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachThueExists(khachThue.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(khachThue);
        }

        // GET: KhachThues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachThue = await _context.KhachThues
                .Include(k => k.PhongTro)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (khachThue == null)
            {
                return NotFound();
            }

            // Chủ trọ chỉ xóa khách của mình
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                var phong = await _context.PhongTros
                    .FirstOrDefaultAsync(p =>
                        p.Id == khachThue.PhongTroId);

                if (phong == null ||
                    phong.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            return View(khachThue);
        }

        // POST: KhachThues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachThue = await _context.KhachThues
                .Include(k => k.PhongTro)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (khachThue == null)
            {
                return NotFound();
            }

            // Chủ trọ chỉ xóa khách của mình
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (khachThue.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            // Xóa user account liên kết
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.KhachThueId == khachThue.Id);

            if (user != null)
            {
                _context.Users.Remove(user);
            }

            var phong = khachThue.PhongTro;

            // Xóa khách thuê
            _context.KhachThues.Remove(khachThue);

            await _context.SaveChangesAsync();

            // Nếu phòng không còn khách
            var soKhach = await _context.KhachThues
                .CountAsync(k => k.PhongTroId == phong.Id);

            if (soKhach == 0)
            {
                phong.TrangThai = "Trống";

                _context.Update(phong);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool KhachThueExists(int id)
        {
            return _context.KhachThues.Any(e => e.Id == id);
        }
    }
}
