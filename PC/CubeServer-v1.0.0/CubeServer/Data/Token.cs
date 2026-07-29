using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CubeServer.Data
{
    public class Token
    {
        public string userId { get; set; }
        public string token { get; set; }
        public DateTime? expiry { get; set; }
        public DateTime? lastAccess { get; set; }

        public int cellId;
    }


    public enum TokenStatus
    {
        TokenBad,
        TokenNotFound,
        TokenExpired,
        TokenOk
    }

    public static class TokenManager
    {
        public static (TokenStatus, Token) Authenticate(HttpContext httpContext)
        {
            string hdrAuth = httpContext.Request.Headers["Authorization"];
            if (hdrAuth == null) return (TokenStatus.TokenNotFound, null);

            string[] s = hdrAuth.Split(Global.spaceSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (s.Length < 2) return (TokenStatus.TokenBad, null);
            if (s[0] != "Bearer") return (TokenStatus.TokenBad, null);

            Token token = Global.db.GetTokenFromTokenStr(s[1]);
            if (token == null) return (TokenStatus.TokenNotFound, null);

            if (DateTime.Compare(DateTime.Now, (DateTime)token.expiry) > 0) return (TokenStatus.TokenExpired, token);

            return (TokenStatus.TokenOk, token);
        }
    }

}
