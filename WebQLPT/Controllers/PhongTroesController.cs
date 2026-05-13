using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebQLPT.Data;
using WebQLPT.Models;
using WebQLPT.Helpers;

namespace WebQLPT.Controllers
{
    [Authorize(Roles = "admin,chutro,khachthue")]
    public class PhongTroesController : Controller
    {
        private readonly AppDbContext _context;

        public PhongTroesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PhongTroes
        public async Task<IActionResult> Index(string keyword)
        {
            var query = _context.PhongTros.Include(p => p.ChuTro).AsQueryable();

            if (User.IsInRole("admin"))
            {
                
            }

          
            else if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                query = query.Where(p => p.ChuTroId == chuTroId);
            }

            
            else if (User.IsInRole("khachthue"))
            {
                var khachThueId = UserHelper.GetKhachThueId(User);

                var khach = await _context.KhachThues
                    .FirstOrDefaultAsync(k => k.Id == khachThueId);

                if (khach != null)
                {
                    query = query.Where(p => p.Id == khach.PhongTroId);
                }
                else
                {
                    query = query.Where(p => false);
                }
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.TenPhong.Contains(keyword));
            }

            return View(await query.ToListAsync());
        }

        // GET: PhongTroes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phongTro = await _context.PhongTros
                .Include(p => p.ChuTro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phongTro == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (phongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

          
            if (User.IsInRole("khachthue"))
            {
                var khachThueId = UserHelper.GetKhachThueId(User);

                var khach = await _context.KhachThues
                    .FirstOrDefaultAsync(k => k.Id == khachThueId);

                if (khach == null || phongTro.Id != khach.PhongTroId)
                {
                    return Forbid();
                }
            }

            return View(phongTro);
        }

        // GET: PhongTroes/Create
        [Authorize(Roles = "admin,chutro")]
        public IActionResult Create()
        {
            ViewData["ChuTroId"] = new SelectList(_context.ChuTros, "Id", "TenChuTro");
            return View();
        }

        // POST: PhongTroes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,chutro")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenPhong,Gia,TrangThai,MoTa")] PhongTro phongTro)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return Content("Lỗi: " + string.Join(" | ", errors));
            }

            if (User.IsInRole("chutro"))
            {
                phongTro.ChuTroId = UserHelper.GetChuTroId(User).Value;
            }

            _context.Add(phongTro);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: PhongTroes/Edit/5
        [Authorize(Roles = "admin,chutro")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var phongTro = await _context.PhongTros.FindAsync(id);
            if (phongTro == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (phongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            ViewData["ChuTroId"] = new SelectList(_context.ChuTros, "Id", "TenChuTro", phongTro.ChuTroId);
            return View(phongTro);
        }

        // POST: PhongTroes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,chutro")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenPhong,Gia,TrangThai,MoTa,ChuTroId")] PhongTro phongTro)
        {
            if (id != phongTro.Id)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (phongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phongTro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongTroExists(phongTro.Id))
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
            ViewData["ChuTroId"] = new SelectList(_context.ChuTros, "Id", "Id", phongTro.ChuTroId);
            return View(phongTro);
        }

        // GET: PhongTroes/Delete/5
        [Authorize(Roles = "admin,chutro")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phongTro = await _context.PhongTros
                .Include(p => p.ChuTro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phongTro == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (phongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            return View(phongTro);
        }

        // POST: PhongTroes/Delete/5
        [Authorize(Roles = "admin,chutro")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phongTro = await _context.PhongTros.FindAsync(id);
            
            if(phongTro == null)
            {
                return NotFound();
            }

            if (User.IsInRole("chutro"))
            {
                var chuTroId = UserHelper.GetChuTroId(User);

                if (phongTro.ChuTroId != chuTroId)
                {
                    return Forbid();
                }
            }

            _context.PhongTros.Remove(phongTro);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhongTroExists(int id)
        {
            return _context.PhongTros.Any(e => e.Id == id);
        }
    }
}
