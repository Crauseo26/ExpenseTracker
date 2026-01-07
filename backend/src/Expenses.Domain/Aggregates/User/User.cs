using Expenses.Domain.Exceptions;

namespace Expenses.Domain.Aggregates.User;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private User()
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(Guid id, string email, string passwordHash)
    {
        if (id == Guid.Empty)
            throw new DomainException(ErrorCodes.InvalidUserId, "User ID cannot be empty");
        
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException(ErrorCodes.InvalidUserEmail, "Email cannot be empty");
        
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(ErrorCodes.InvalidPasswordHash, "Password hash cannot be empty");

        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (DeletedAt.HasValue)
            throw new DomainException(ErrorCodes.UserAlreadyDeleted, "User is already deleted");

        DeletedAt = DateTime.UtcNow;
    }

    public bool IsDeleted => DeletedAt.HasValue;
}
