namespace Expenses.Domain.Exceptions;

public static class ErrorCodes
{
    // User errors: 1xxx
    public const string UserAlreadyDeleted = "DOM1001";
    public const string InvalidUserId = "DOM1002";
    public const string InvalidUserEmail = "DOM1003";
    public const string InvalidPasswordHash = "DOM1004";
}
