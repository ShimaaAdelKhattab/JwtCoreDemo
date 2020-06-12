using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtCoreDemo.Managers
{
    public interface IReaderRepo
    {
        List<User> Users { get; }
        AuthResult Authenticate(LoginModel credentials);
    }

    public class ReaderRepo : IReaderRepo
    {
        private static List<User> users;
        private ITokenManager _tokenManager;

        public ReaderRepo(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager;
            SeedUsers();
        }

        private void SeedUsers()
        {
            users = new List<User>() {
            new User {
                Id = 1,
               Role  = "x",
               EmailAddress  = "reader1@me.com"
            }
        };
        }

        public List<User> Users => users;

        public AuthResult Authenticate(LoginModel credentials)
        {
            var user = users.FirstOrDefault(x => x.EmailAddress == credentials.Email);

            if (user != null)
            { 
                var x = new AuthResult
                {
                    IsSuccess = true,
                    Token = _tokenManager.Generate(user)
                };
                return new AuthResult
                {
                    IsSuccess = true,
                    Token = _tokenManager.Generate(user)
                };
            }

            return new AuthResult { IsSuccess = false };
        }
    }
}
