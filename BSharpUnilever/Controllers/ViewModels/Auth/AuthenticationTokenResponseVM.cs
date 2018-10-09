using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers.ViewModels.Auth
{
    /// <summary>
    /// Represents the authentication token and some of its content
    /// </summary>
    public class AuthenticationTokenResponseVM
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }

        public string Email { get; set; }

        public string Jti { get; set; }
    }
}
