using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQLPT.Data;
using WebQLPT.Models;
using WebQLPT.Services;

namespace WebQLPT.Controllers
{
    [Authorize]
    public class VnPayController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly AppDbContext _context;

        public VnPayController(
            IVnPayService vnPayService,
            AppDbContext context)
        {
            _vnPayService = vnPayService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(int hoaDonId)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.KhachThue)
                .FirstOrDefaultAsync(h => h.Id == hoaDonId);

            if (hoaDon == null)
            {
                return NotFound();
            }

            var model = new VnPaymentRequestModel
            {
                HoaDonId = hoaDon.Id,
                FullName = hoaDon.KhachThue?.TenKhach,
                Description = $"Thanh-toan-hoa-don-{hoaDon.Id}",
                Amount = hoaDon.TongTien,
                CreatedDate = DateTime.Now
            };

            var paymentUrl =
                _vnPayService.CreatePaymentUrl(
                    HttpContext,
                    model);
            Console.WriteLine("=== PAYMENT URL ===");
            Console.WriteLine(paymentUrl);

            return Redirect(paymentUrl);
        }

        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            Console.WriteLine("=== CALLBACK SUCCESS === " + response.Success);

            if (response.Success)
            {
                int hoaDonId = int.Parse(response.OrderId);

                var hoaDon = await _context.HoaDons.FindAsync(hoaDonId);

                if (hoaDon != null)
                {
                    hoaDon.TrangThai = "Đã thanh toán";
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Thanh toán VNPay thành công";
            }
            else
            {
                TempData["Error"] = "Thanh toán thất bại (Sai chữ ký hoặc bị reject)";
            }

            return RedirectToAction("MyBills", "HoaDons");
        }

    }
}
