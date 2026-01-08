using Microsoft.AspNetCore.Identity;
using Expenses.Domain.Exceptions;

namespace Expenses.Domain.Aggregates.User;

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public User()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public User(string email) : this()
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException(ErrorCodes.InvalidUserEmail, "Email cannot be empty");

        Email = email;
        UserName = email;
        NormalizedEmail = email.ToUpperInvariant();
        NormalizedUserName = email.ToUpperInvariant();
    }

    public void SoftDelete()
    {
        if (DeletedAt.HasValue)
            throw new DomainException(ErrorCodes.UserAlreadyDeleted, "User is already deleted");

        DeletedAt = DateTime.UtcNow;
    }

    public bool IsDeleted => DeletedAt.HasValue;
}
