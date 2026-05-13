using System.Security.Claims;

namespace WebQLPT.Helpers
{
    public class UserHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            return int.Parse(
                user.FindFirst("UserId")!.Value);
        }

        public static int? GetChuTroId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst("ChuTroId");

            if (claim == null)
                return null;

            return int.Parse(claim.Value);
        }

        public static int? GetKhachThueId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst("KhachThueId");

            if (claim == null)
                return null;

            return int.Parse(claim.Value);
        }
    }
}
