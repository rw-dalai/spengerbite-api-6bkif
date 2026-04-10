using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models;

public class UserAccount : EntityBase
{
    public Email Email { get; private set; }

    public PasswordHash PasswordHash { get; private set; }

    public UserRole Role { get; private set; }

    public bool IsEnabled { get; private set; }


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
