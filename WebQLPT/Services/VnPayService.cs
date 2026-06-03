using Microsoft.Extensions.Options;
using WebQLPT.Configuration;
using WebQLPT.Libraries;
using WebQLPT.Models;
using WebQLPT.Services;

public class VnPayService : IVnPayService
{
    private readonly VnPayConfig _config;

    public VnPayService(IOptions<VnPayConfig> config)
    {
        _config = config.Value;
    }

    public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
    {
        var vnpay = new VnPayLibrary();

        vnpay.AddRequestData("vnp_Version", "2.1.1");
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", _config.TmnCode);

        vnpay.AddRequestData("vnp_Amount", ((long)(model.Amount * 100)).ToString());
        vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

        vnpay.AddRequestData("vnp_CurrCode", "VND");
        var ipAddr = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        // Nếu là IPv6 loopback (::1) thì đổi sang IPv4
        if (ipAddr == "::1") ipAddr = "127.0.0.1";

        vnpay.AddRequestData("vnp_IpAddr", ipAddr);
        vnpay.AddRequestData("vnp_Locale", "vn");

        vnpay.AddRequestData("vnp_OrderInfo", RemoveDiacritics(model.Description));
        vnpay.AddRequestData("vnp_OrderType", "other");

        vnpay.AddRequestData("vnp_ReturnUrl", _config.ReturnUrl);
        vnpay.AddRequestData("vnp_TxnRef", model.HoaDonId.ToString());

        return vnpay.CreateRequestUrl(_config.BaseUrl, _config.HashSecret);
    }

    private string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder();
        foreach (var c in normalized)
        {
            var cat = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (cat != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
    }

    public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
    {
        var vnpay = new VnPayLibrary();

        var isValid = vnpay.ValidateSignature(collections, _config.HashSecret);

        return new VnPaymentResponseModel
        {
            OrderId = collections["vnp_TxnRef"],
            TransactionId = collections["vnp_TransactionNo"],
            OrderDescription = collections["vnp_OrderInfo"],
            VnPayResponseCode = collections["vnp_ResponseCode"],
            Success = isValid && collections["vnp_ResponseCode"] == "00"
        };
    }
}