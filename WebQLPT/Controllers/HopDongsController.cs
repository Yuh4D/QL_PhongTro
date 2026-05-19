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
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro);
            return View(await appDbContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hopDong == null)
            {
                return NotFound();
            }

            return View(hopDong);
        }

        // GET: HopDongs/Create
        public IActionResult Create()
        {
            ViewData["KhachThueId"] = new SelectList(_context.KhachThues, "Id", "TenKhach");
            ViewData["PhongTroId"] = new SelectList(_context.PhongTros, "Id", "TenPhong");
            return View();
        }

        // POST: HopDongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PhongTroId,KhachThueId,NgayBatDau,NgayKetThuc,TienCoc,NoiDung")] HopDong hopDong)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
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

                var chuTro = phong.ChuTro;
                hopDong.NoiDung = _service.GenerateHopDong(hopDong, phong, khach, chuTro);

                _context.Add(hopDong);

                phong.TrangThai = "Đã thuê";
                _context.Update(phong);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = hopDong.Id });
            }

            ViewData["KhachThueId"] = new SelectList(_context.KhachThues, "Id", "TenKhach", hopDong.KhachThueId);
            ViewData["PhongTroId"] = new SelectList(_context.PhongTros, "Id", "TenPhong", hopDong.PhongTroId);
            return View(hopDong);
        }

        // GET: HopDongs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null)
            {
                return NotFound();
            }
            ViewData["KhachThueId"] = new SelectList(_context.KhachThues, "Id", "TenKhach", hopDong.KhachThueId);
            ViewData["PhongTroId"] = new SelectList(_context.PhongTros, "Id", "TenPhong", hopDong.PhongTroId);
            return View(hopDong);
        }

        // POST: HopDongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PhongTroId,KhachThueId,NgayBatDau,NgayKetThuc,TienCoc,NoiDung")] HopDong hopDong)
        {
            if (id != hopDong.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
             
                    var phong = await _context.PhongTros
                        .Include(p => p.ChuTro)
                        .FirstOrDefaultAsync(p => p.Id == hopDong.PhongTroId);

                    var khach = await _context.KhachThues
                        .FirstOrDefaultAsync(k => k.Id == hopDong.KhachThueId);

                    var chuTro = phong.ChuTro;

                    hopDong.NoiDung = _service.GenerateHopDong(hopDong, phong, khach, chuTro);

                    _context.Update(hopDong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HopDongExists(hopDong.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["KhachThueId"] = new SelectList(_context.KhachThues, "Id", "TenKhach", hopDong.KhachThueId);
            ViewData["PhongTroId"] = new SelectList(_context.PhongTros, "Id", "TenPhong", hopDong.PhongTroId);
            return View(hopDong);
        }

        // GET: HopDongs/Delete/5
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

            return View(hopDong);
        }

        // POST: HopDongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong != null)
            {
                _context.HopDongs.Remove(hopDong);
            }

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
    }
}
