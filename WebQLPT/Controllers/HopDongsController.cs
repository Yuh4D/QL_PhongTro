using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebQLPT.Data;
using WebQLPT.Helpers;
using WebQLPT.Models;
using WebQLPT.Services;

namespace WebQLPT.Controllers
{
    [Authorize(Roles = "admin,chutro,khachthue")]
    public class HopDongsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly HopDongService _service;

        public HopDongsController(AppDbContext context)
        {
            _context = context;
            _service = new HopDongService();
        }

        // GET: HopDongs

        public async Task<IActionResult> Index(string keyword)
        {
            var query = _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.ChuTro)
                .AsQueryable();

            if (User.IsInRole("admin"))
            {
                // no filter
            }
            else if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);
                query = query.Where(h =>
                    h.PhongTro != null &&
                    h.PhongTro.ChuTroId == chuTroId);
            }
            else if (User.IsInRole("khachthue"))
            {
                var khachId = UserHelper.GetKhachThueId(User);
                query = query.Where(h => h.KhachThueId == khachId);
            }
            else
            {
                return Forbid();
            }

            var list = await query.ToListAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();

                list = list.Where(h =>
                    (h.PhongTro != null && (h.PhongTro.TenPhong ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (h.KhachThue != null && (h.KhachThue.TenKhach ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    h.NgayBatDau.ToString("dd/MM/yyyy").Contains(keyword) ||
                    h.NgayKetThuc.ToString("dd/MM/yyyy").Contains(keyword) ||
                    h.TienCoc.ToString().Contains(keyword) ||
                    h.TienCoc.ToString("N0", new System.Globalization.CultureInfo("vi-VN")).Contains(keyword)
                ).ToList();
            }

            return View(list);
        }

        // GET: HopDongs/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDong = await _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hopDong == null)
            {
                return NotFound();
            }

            // ADMIN
            if (User.IsInRole("admin"))
            {
                return View(hopDong);
            }

            // CHỦ TRỌ
            if (User.IsInRole("chutro"))
            {
                var chuTroId =
                    UserHelper.GetChuTroId(User);

                var isOwner = await _context.PhongTros
                    .AnyAsync(p =>
                        p.Id == hopDong.PhongTroId &&
                        p.ChuTroId == chuTroId);

                if (!isOwner)
                {
                    return Forbid();
                }

                return View(hopDong);
            }

            // KHÁCH THUÊ
            if (User.IsInRole("khachthue"))
            {
                var khachId =
                    UserHelper.GetKhachThueId(User);

                if (hopDong.KhachThueId != khachId)
                {
                    return Forbid();
                }

                return View(hopDong);
            }

            return Forbid();
        }

        // GET: HopDongs/Create
        [Authorize(Roles = "admin,chutro")]
        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        // POST: HopDongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,chutro")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PhongTroId,KhachThueId,NgayBatDau,NgayKetThuc,TienCoc,DieuKhoanThem")] HopDong hopDong)
        {
            var phong = await _context.PhongTros
                .Include(p => p.ChuTro)
                .FirstOrDefaultAsync(p => p.Id == hopDong.PhongTroId);

            var khach = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.Id == hopDong.KhachThueId);

            if (phong == null || khach == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (chuTroId == null ||
                    phong.ChuTroId != chuTroId.Value ||
                    khach.PhongTroId != phong.Id)
                {
                    return Forbid();
                }
            }

            if (khach.PhongTroId != phong.Id)
            {
                ModelState.AddModelError(
                    nameof(HopDong.KhachThueId),
                    "Khách thuê không thuộc phòng đã chọn.");
            }

            if (ModelState.IsValid)
            {
                var chuTro = phong.ChuTro;

                if (chuTro == null)
                {
                    return NotFound();
                }

                hopDong.NoiDung = _service.GenerateHopDong(hopDong, phong, khach, chuTro);

                _context.Add(hopDong);

                phong.TrangThai = "Đã thuê";
                _context.Update(phong);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = hopDong.Id });
            }

            PopulateSelectLists(hopDong.PhongTroId, hopDong.KhachThueId);
            return View(hopDong);
        }

        // GET: HopDongs/Edit/5
        [Authorize(Roles = "admin,chutro")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDong = await _context.HopDongs
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hopDong == null)
            {
                return NotFound();
            }

           
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (hopDong.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            PopulateSelectLists(hopDong.PhongTroId, hopDong.KhachThueId);
            return View(hopDong);
        }

        // POST: HopDongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,chutro")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,PhongTroId,KhachThueId,NgayBatDau,NgayKetThuc,TienCoc,DieuKhoanThem")]
    HopDong hopDong)
        {
            if (id != hopDong.Id)
            {
                return NotFound();
            }

           
            var oldHopDong = await _context.HopDongs
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (oldHopDong == null)
            {
                return NotFound();
            }

         
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (chuTroId == null ||
                    oldHopDong.PhongTro.ChuTroId != chuTroId.Value)
                {
                    return Forbid();
                }
            }

            var phong = await _context.PhongTros
                .Include(p => p.ChuTro)
                .FirstOrDefaultAsync(p => p.Id == hopDong.PhongTroId);

            var khach = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.Id == hopDong.KhachThueId);

            if (phong == null || khach == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (chuTroId == null ||
                    phong.ChuTroId != chuTroId.Value ||
                    khach.PhongTroId != phong.Id)
                {
                    return Forbid();
                }
            }

            if (khach.PhongTroId != phong.Id)
            {
                ModelState.AddModelError(
                    nameof(HopDong.KhachThueId),
                    "Khách thuê không thuộc phòng đã chọn.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var chuTro = phong.ChuTro;

                    if (chuTro == null)
                    {
                        return NotFound();
                    }

                    oldHopDong.PhongTroId = hopDong.PhongTroId;
                    oldHopDong.KhachThueId = hopDong.KhachThueId;
                    oldHopDong.NgayBatDau = hopDong.NgayBatDau;
                    oldHopDong.NgayKetThuc = hopDong.NgayKetThuc;
                    oldHopDong.TienCoc = hopDong.TienCoc;
                    oldHopDong.DieuKhoanThem = hopDong.DieuKhoanThem;
                    oldHopDong.NoiDung =
                        _service.GenerateHopDong(
                            oldHopDong,
                            phong,
                            khach,
                            chuTro);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HopDongExists(hopDong.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(hopDong.PhongTroId, hopDong.KhachThueId);
            return View(hopDong);
        }

        // GET: HopDongs/Delete/5
        [Authorize(Roles = "admin,chutro")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDong = await _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hopDong == null)
            {
                return NotFound();
            }

            // CHỦ TRỌ chỉ được xóa hợp đồng của mình
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (hopDong.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            return View(hopDong);
        }

        // POST: HopDongs/Delete/5
        [Authorize(Roles = "admin,chutro")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hopDong = await _context.HopDongs
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hopDong == null)
            {
                return NotFound();
            }

            // CHỦ TRỌ chỉ được xóa hợp đồng của mình
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (hopDong.PhongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            _context.HopDongs.Remove(hopDong);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool HopDongExists(int id)
        {
            return _context.HopDongs.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ExportPdf(int id)
        {
            var hopDong = await _context.HopDongs
                .Include(h => h.PhongTro)
                .Include(h => h.KhachThue)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hopDong == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (chuTroId == null ||
                    hopDong.PhongTro?.ChuTroId != chuTroId.Value)
                {
                    return Forbid();
                }
            }

            if (User.IsInRole("khachthue"))
            {
                var khachThueId = UserHelper.GetKhachThueId(User);

                if (khachThueId == null ||
                    hopDong.KhachThueId != khachThueId.Value)
                {
                    return Forbid();
                }
            }

            return new ViewAsPdf("Pdf", hopDong)
            {
                FileName = $"HopDong_{id}.pdf"
            };
        }

        [Authorize(Roles = "khachthue")]
        public async Task<IActionResult> MyContracts()
        {
            var khachThueId = UserHelper.GetKhachThueId(User);

            var hopDongs = await _context.HopDongs
                .Include(h => h.PhongTro)
                .Where(h => h.KhachThueId == khachThueId)
                .ToListAsync();

            return View(hopDongs);
        }

        private void PopulateSelectLists(
            int? phongTroId = null,
            int? khachThueId = null)
        {
            var phongTros = _context.PhongTros.AsQueryable();
            var khachThues = _context.KhachThues
                .Include(k => k.PhongTro)
                .AsQueryable();

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                phongTros = phongTros
                    .Where(p => p.ChuTroId == chuTroId);

                khachThues = khachThues
                    .Where(k => k.PhongTro != null &&
                                k.PhongTro.ChuTroId == chuTroId);
            }

            ViewData["PhongTroId"] = new SelectList(
                phongTros,
                "Id",
                "TenPhong",
                phongTroId);

            ViewData["KhachThueId"] = new SelectList(
                khachThues,
                "Id",
                "TenKhach",
                khachThueId);
        }
    }
}
