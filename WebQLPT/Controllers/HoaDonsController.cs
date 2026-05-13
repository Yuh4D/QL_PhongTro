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
using WebQLPT.Models;

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
            var appDbContext = _context.HoaDons.Include(h => h.KhachThue).Include(h => h.PhongTro);
            return View(await appDbContext.ToListAsync());
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

            return View(hoaDon);
        }

        // GET: HoaDons/Create
        public IActionResult Create()
        {
            ViewData["PhongTroId"] = new SelectList(_context.PhongTros, "Id", "TenPhong");
            return View();
        }

        // POST: HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoaDon hoaDon)
        {
            if (!ModelState.IsValid)
            {
                ViewData["PhongTroId"] = new SelectList(_context.PhongTros, "Id", "TenPhong", hoaDon.PhongTroId);
                return View(hoaDon);
       
            }

            //Lấy khách theo phòng
            var khach = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.PhongTroId == hoaDon.PhongTroId);

            if (khach == null)
            {
                TempData["Error"] = "Phòng chưa có khách thuê!";
                return RedirectToAction(nameof(Create));
            }

            //Gán dữ liệu tự động
            hoaDon.KhachThueId = khach.Id;
            hoaDon.TrangThai = "Mới tạo";

            _context.Add(hoaDon);
            await _context.SaveChangesAsync();

            decimal tong = 0;

            if (hoaDon.ChiTiets != null)
            {
                foreach (var ct in hoaDon.ChiTiets)
                {
                    if (string.IsNullOrEmpty(ct.NoiDung)) continue;

                    ct.HoaDonId = hoaDon.Id;

                    if (ct.HeSo == 0) ct.HeSo = 1;

                    ct.ThanhTien = ct.DonGia * ct.SoLuong * ct.HeSo;

                    tong += ct.ThanhTien;

                    _context.HoaDonChiTiets.Add(ct);
                }
            }

            hoaDon.TongTien = tong;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = hoaDon.Id });
        }

        // GET: HoaDons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewData["KhachThueId"] = new SelectList(_context.KhachThues, "Id", "Id", hoaDon.KhachThueId);
            ViewData["PhongTroId"] = new SelectList(_context.PhongTros, "Id", "Id", hoaDon.PhongTroId);
            return View(hoaDon);
        }

        // POST: HoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PhongTroId,KhachThueId,NgayTao,HanThanhToan,TongTien,TrangThai")] HoaDon hoaDon)
        {
            if (id != hoaDon.Id)
            {
                return NotFound();
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
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["KhachThueId"] = new SelectList(_context.KhachThues, "Id", "Id", hoaDon.KhachThueId);
            ViewData["PhongTroId"] = new SelectList(_context.PhongTros, "Id", "Id", hoaDon.PhongTroId);
            return View(hoaDon);
        }

        // GET: HoaDons/Delete/5
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

            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }

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

        // POST : HoaDons/AddDetail
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddDetail([Bind("HoaDonId,NoiDung,DonGia,SoLuong,HeSo")] HoaDonChiTiet ct)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.HoaDonId = ct.HoaDonId;
        //        return View(ct);

        //    }

        //    if (ct.HeSo == 0)
        //        ct.HeSo = 1;

        //    ct.ThanhTien = ct.DonGia * ct.SoLuong * ct.HeSo;

        //    _context.HoaDonChiTiets.Add(ct);
        //    await _context.SaveChangesAsync();

        //    var tongTien = await _context.HoaDonChiTiets
        //        .Where(x => x.HoaDonId == ct.HoaDonId)
        //        .SumAsync(x => x.ThanhTien);

        //    var hoaDon = await _context.HoaDons.FindAsync(ct.HoaDonId);
        //    hoaDon.TongTien = tongTien;

        //    _context.Update(hoaDon);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Details), new { id = ct.HoaDonId });
        //}

        public async Task<IActionResult> ExportPdf(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.ChuTro)
                .Include(h => h.KhachThue)
                .Include(h => h.HoaDonChiTiets)
                .FirstOrDefaultAsync(h => h.Id == id);

            return new ViewAsPdf("Pdf", hoaDon)
            {
                FileName = $"HoaDon_{id}.pdf"
            };
        }
    }
}
