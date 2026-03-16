using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models.Auth;

public class UserAccount : EntityBase
{
    public Email Email { get; set; }

    public PasswordHash PasswordHash { get; set; }

    public UserRole Role { get; set; }

    public bool IsEnabled { get; set; }


    // EF Core
    protected UserAccount() { }

    // Business Ctor
    public UserAccount(Email email, PasswordHash passwordHash, UserRole role, bool isEnabled)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        IsEnabled = isEnabled;
    }
}
