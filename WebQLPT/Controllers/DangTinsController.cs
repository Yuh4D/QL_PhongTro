using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebQLPT.Data;
using WebQLPT.Helpers;
using WebQLPT.Models;

namespace WebQLPT.Controllers
{
    [Authorize]
    public class DangTinsController : Controller
    {
        private readonly AppDbContext _context;

        public DangTinsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DangTins
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? keyword)
        {
            var query = _context.DangTins
                .Include(d => d.ChuTro)
                .Include(d => d.PhongTro)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(d =>
                    d.TieuDe.Contains(keyword) ||
                    d.NoiDung.Contains(keyword) ||
                    (d.PhongTro != null &&
                     d.PhongTro.TenPhong != null &&
                     d.PhongTro.TenPhong.Contains(keyword)) ||
                    (d.ChuTro != null &&
                     d.ChuTro.TenChuTro != null &&
                     d.ChuTro.TenChuTro.Contains(keyword)));
            }

            ViewBag.Keyword = keyword;

            return View(await query
                .OrderByDescending(d => d.NgayDang)
                .ToListAsync());
        }

        // GET: DangTins/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangTin = await _context.DangTins
                .Include(d => d.ChuTro)
                .Include(d => d.PhongTro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dangTin == null)
            {
                return NotFound();
            }

            return View(dangTin);
        }

        // GET: DangTins/Create
        [Authorize(Roles = "admin,chutro")]
        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        // POST: DangTins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin,chutro")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TieuDe,NoiDung,Gia,HinhAnh,PhongTroId,ChuTroId")] DangTin dangTin, IFormFile? ImageFile)
        {
            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (chuTroId == null)
                {
                    return Forbid();
                }

                var phongThuocChuTro = await _context.PhongTros
                    .AnyAsync(p => p.Id == dangTin.PhongTroId &&
                                   p.ChuTroId == chuTroId.Value);

                if (!phongThuocChuTro)
                {
                    return Forbid();
                }

                dangTin.ChuTroId = chuTroId.Value;
                ModelState.Remove(nameof(DangTin.ChuTroId));
            }

            if (ModelState.IsValid)
            {
                dangTin.NgayDang = DateTime.Now;

                // Upload ảnh
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName =
                        Guid.NewGuid().ToString() +
                        Path.GetExtension(ImageFile.FileName);

                    var uploadPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads",
                        fileName);

                    using (var stream = new FileStream(
                        uploadPath,
                        FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    dangTin.HinhAnh = "/uploads/" + fileName;
                }

                _context.Add(dangTin);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(dangTin.PhongTroId, dangTin.ChuTroId);
            return View(dangTin);
        }

        // GET: DangTins/Edit/5
        [Authorize(Roles = "admin,chutro")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangTin = await _context.DangTins.FindAsync(id);
            if (dangTin == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (chuTroId == null || dangTin.ChuTroId != chuTroId.Value)
                {
                    return Forbid();
                }
            }

            PopulateSelectLists(dangTin.PhongTroId, dangTin.ChuTroId);
            return View(dangTin);
        }

        // POST: DangTins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin,chutro")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TieuDe,NoiDung,Gia,HinhAnh,NgayDang,PhongTroId,ChuTroId")] DangTin dangTin, IFormFile? ImageFile)
        {
            if (id != dangTin.Id)
            {
                return NotFound();
            }

            // Lấy dữ liệu cũ
            var oldDangTin = await _context.DangTins
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (oldDangTin == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (chuTroId == null || oldDangTin.ChuTroId != chuTroId.Value)
                {
                    return Forbid();
                }

                var phongThuocChuTro = await _context.PhongTros
                    .AnyAsync(p => p.Id == dangTin.PhongTroId &&
                                   p.ChuTroId == chuTroId.Value);

                if (!phongThuocChuTro)
                {
                    return Forbid();
                }

                dangTin.ChuTroId = chuTroId.Value;
                ModelState.Remove(nameof(DangTin.ChuTroId));
            }

            // Nếu không upload ảnh mới -> giữ ảnh cũ
            if (ImageFile == null)
            {
                dangTin.HinhAnh = oldDangTin.HinhAnh;
            }
            else
            {
                // Tạo tên file unique
                string fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(ImageFile.FileName);

                // Đường dẫn lưu file
                string uploadFolder =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads");

                // Tạo folder nếu chưa có
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string filePath =
                    Path.Combine(uploadFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // Lưu đường dẫn DB
                dangTin.HinhAnh = "/uploads/" + fileName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dangTin);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DangTinExists(dangTin.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(dangTin.PhongTroId, dangTin.ChuTroId);
            return View(dangTin);
        }

        // GET: DangTins/Delete/5
        [Authorize(Roles = "admin,chutro")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangTin = await _context.DangTins
                .Include(d => d.ChuTro)
                .Include(d => d.PhongTro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dangTin == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (chuTroId == null || dangTin.ChuTroId != chuTroId.Value)
                {
                    return Forbid();
                }
            }

            return View(dangTin);
        }

        // POST: DangTins/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin,chutro")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dangTin = await _context.DangTins.FindAsync(id);

            if (dangTin == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (chuTroId == null || dangTin.ChuTroId != chuTroId.Value)
                {
                    return Forbid();
                }
            }

            _context.DangTins.Remove(dangTin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DangTinExists(int id)
        {
            return _context.DangTins.Any(e => e.Id == id);
        }

        private void PopulateSelectLists(int? phongTroId = null, int? chuTroId = null)
        {
            var phongTros = _context.PhongTros.AsQueryable();

            if (User.IsInRole("chutro"))
            {
                var currentChuTroId = UserHelper.GetChuTroId(User);
                phongTros = phongTros.Where(p => p.ChuTroId == currentChuTroId);
                chuTroId = currentChuTroId;
            }

            ViewBag.PhongTroId = new SelectList(
                phongTros,
                "Id",
                "TenPhong",
                phongTroId);

            ViewBag.ChuTroId = new SelectList(
                _context.ChuTros,
                "Id",
                "TenChuTro",
                chuTroId);
        }
    }
}
