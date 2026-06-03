using System.Security.Cryptography;
using System.Text;

namespace WebQLPT.Libraries
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData
            = new SortedList<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                _requestData[key] = value;
        }

        public string CreateRequestUrl(string baseUrl, string secretKey)
        {
            var encodedParts = _requestData
                .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}");

            var encodedQuery = string.Join("&", encodedParts);

            var hash = HmacSHA512(secretKey, encodedQuery);

            return $"{baseUrl}?{encodedQuery}&vnp_SecureHashType=HmacSHA512&vnp_SecureHash={hash}";
        }

        public bool ValidateSignature(IQueryCollection query, string secretKey)
        {
            var rawPartsEncoded = query
                .Where(x => x.Key.StartsWith("vnp_"))
                .Where(x => x.Key != "vnp_SecureHash" && x.Key != "vnp_SecureHashType")
                .OrderBy(x => x.Key, StringComparer.InvariantCultureIgnoreCase)
                .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value.ToString())}");

            var raw = string.Join("&", rawPartsEncoded);

            var inputHash = query["vnp_SecureHash"].ToString();
            var computedHash = HmacSHA512(secretKey, raw);

            Console.WriteLine("RAW: " + raw);
            Console.WriteLine("INPUT:    " + inputHash);
            Console.WriteLine("COMPUTED: " + computedHash);

            return string.Equals(inputHash, computedHash, StringComparison.OrdinalIgnoreCase);
        }

        private string HmacSHA512(string key, string inputData)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}