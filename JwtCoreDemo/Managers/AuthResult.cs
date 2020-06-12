using static JwtCoreDemo.Managers.TokenManager;

namespace JwtCoreDemo.Managers
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public AuthToken Token { get; set; }

    }
}