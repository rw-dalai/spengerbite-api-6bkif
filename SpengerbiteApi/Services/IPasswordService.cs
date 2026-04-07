using SpengerbiteApi.Models.Auth;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Services;

public interface IPasswordService
{
    PasswordHash HashPassword(string plaintext);
    bool VerifyPassword(UserAccount account, string plaintext);
}
