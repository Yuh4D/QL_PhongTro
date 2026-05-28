using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using WebQLPT.Data;
using WebQLPT.Models;
using WebQLPT.Helpers;

namespace WebQLPT.Controllers
{
    [Authorize(Roles = "admin,chutro,khachthue")]
    public class HoaDonsController : Controller
    {
        private readonly AppDbContext _context;

        public HoaDonsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: HoaDons
        public async Task<IActionResult> Index()
        {
            var query = _context.HoaDons
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.ChuTro)
                .AsQueryable();

      
            if (User.IsInRole("admin"))
            {
                return View(await query.ToListAsync());
            }

      
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                query = query.Where(h =>
                    h.PhongTro.ChuTroId == chuTroId);

                return View(await query.ToListAsync());
            }

         
            if (User.IsInRole("khachthue"))
            {
                var khachThueId = UserHelper.GetKhachThueId(User);

                query = query.Where(h =>
                    h.KhachThueId == khachThueId);

                return View(await query.ToListAsync());
            }

            return Forbid();
        }

        // GET: HoaDons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.ChuTro)
                .Include(h => h.HoaDonChiTiets)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

        
            if (User.IsInRole("admin"))
            {
                return View(hoaDon);
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (hoaDon.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }

                return View(hoaDon);
            }

            
            if (User.IsInRole("khachthue"))
            {
                var khachThueId = UserHelper.GetKhachThueId(User);

                if (hoaDon.KhachThueId != khachThueId)
                {
                    return Forbid();
                }

                return View(hoaDon);
            }

            return Forbid();
        }

        // GET: HoaDons/Create
        [Authorize(Roles = "admin,chutro")]
        public IActionResult Create()
        {
            // ADMIN
            if (User.IsInRole("admin"))
            {
                ViewData["PhongTroId"] =
                    new SelectList(_context.PhongTros,
                        "Id",
                        "TenPhong");

                return View();
            }

          
            var chuTroId = UserHelper.GetChuTroId(User);

            var phongTroList = _context.PhongTros
                .Where(p => p.ChuTroId == chuTroId)
                .ToList();

            ViewData["PhongTroId"] =
                new SelectList(phongTroList,
                    "Id",
                    "TenPhong");

            return View();
        }

        // POST: HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,chutro")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoaDon hoaDon)
        {
            // Kiểm tra phòng tồn tại
            var phongTro = await _context.PhongTros
                .FirstOrDefaultAsync(p => p.Id == hoaDon.PhongTroId);

            if (phongTro == null)
            {
                return NotFound();
            }

            // CHỦ TRỌ chỉ được tạo hóa đơn phòng mình
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (phongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["PhongTroId"] =
                    new SelectList(_context.PhongTros,
                        "Id",
                        "TenPhong",
                        hoaDon.PhongTroId);

                return View(hoaDon);
            }

            // Lấy khách thuê theo phòng
            var khach = await _context.KhachThues
                .FirstOrDefaultAsync(k =>
                    k.PhongTroId == hoaDon.PhongTroId);

            if (khach == null)
            {
                TempData["Error"] = "Phòng chưa có khách thuê!";

                return RedirectToAction(nameof(Create));
            }

            // Gán tự động
            hoaDon.KhachThueId = khach.Id;

            hoaDon.TrangThai = "Mới tạo";

            _context.Add(hoaDon);

            await _context.SaveChangesAsync();

            decimal tong = 0;

            if (hoaDon.ChiTiets != null)
            {
                foreach (var ct in hoaDon.ChiTiets)
                {
                    if (string.IsNullOrEmpty(ct.NoiDung))
                        continue;

                    ct.HoaDonId = hoaDon.Id;

                    if (ct.HeSo == 0)
                        ct.HeSo = 1;

                    ct.ThanhTien =
                        ct.DonGia *
                        ct.SoLuong *
                        ct.HeSo;

                    tong += ct.ThanhTien;

                    _context.HoaDonChiTiets.Add(ct);
                }
            }

            hoaDon.TongTien = tong;

            await _context.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                new { id = hoaDon.Id });
        }

        // GET: HoaDons/Edit/5
        [Authorize(Roles = "admin,chutro")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

   
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (hoaDon.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            ViewData["KhachThueId"] =
                new SelectList(_context.KhachThues,
                    "Id",
                    "Id",
                    hoaDon.KhachThueId);

            ViewData["PhongTroId"] =
                new SelectList(_context.PhongTros,
                    "Id",
                    "TenPhong",
                    hoaDon.PhongTroId);

            return View(hoaDon);
        }

        // POST: HoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,chutro")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,PhongTroId,KhachThueId,NgayTao,HanThanhToan,TongTien,TrangThai")]
            HoaDon hoaDon)
        {
            if (id != hoaDon.Id)
            {
                return NotFound();
            }

            var oldHoaDon = await _context.HoaDons
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (oldHoaDon == null)
            {
                return NotFound();
            }

          
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (oldHoaDon.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDon);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(hoaDon);
        }

        // GET: HoaDons/Delete/5
        [Authorize(Roles = "admin,chutro")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

        
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (hoaDon.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [Authorize(Roles = "admin,chutro")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

     
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (hoaDon.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            _context.HoaDons.Remove(hoaDon);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.Id == id);
        }

        // GET: HoaDons/AddDetail
        public IActionResult AddDetail(int id)
        {
            var hoaDon = _context.HoaDons.Find(id);
            if (hoaDon == null) return NotFound();

            ViewBag.HoaDonId = id;
            return View();
        }


        public async Task<IActionResult> ExportPdf(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.ChuTro)
                .Include(h => h.KhachThue)
                .Include(h => h.HoaDonChiTiets)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

     
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (hoaDon.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            if (User.IsInRole("khachthue"))
            {
                var khachThueId = UserHelper.GetKhachThueId(User);

                if (hoaDon.KhachThueId != khachThueId)
                {
                    return Forbid();
                }
            }

            return new ViewAsPdf("Pdf", hoaDon)
            {
                FileName = $"HoaDon_{id}.pdf"
            };
        }

        [Authorize(Roles = "khachthue")]
        public async Task<IActionResult> MyBills()
        {
            var khachThueId = UserHelper.GetKhachThueId(User);

            var hoaDons = await _context.HoaDons
                .Include(h => h.PhongTro)
                .Where(h => h.KhachThueId == khachThueId)
                .ToListAsync();

            return View(hoaDons);
        }

        // GET: HoaDons/Payment/5
        public async Task<IActionResult> Payment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.PhongTro)
                .Include(h => h.KhachThue)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        [Authorize(Roles = "khachthue")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            var khachThueId = UserHelper.GetKhachThueId(User);

            if (hoaDon.KhachThueId != khachThueId)
            {
                return Forbid();
            }

            hoaDon.TrangThai = "Đã thanh toán";

            _context.Update(hoaDon);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Thanh toán thành công";

            return RedirectToAction(nameof(MyBills));
        }
    }
}
