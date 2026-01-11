namespace Expenses.Domain.Exceptions;

public static class ErrorCodes
{
    // User errors: 1xxx
    public const string UserAlreadyDeleted = "DOM1001";
    public const string InvalidUserId = "DOM1002";
    public const string InvalidUserEmail = "DOM1003";
    public const string InvalidPasswordHash = "DOM1004";

    // Expense errors: 2xxx
    public const string InvalidExpenseTransition = "DOM2001";
    public const string InvalidMoneyAmount = "DOM2002";
    public const string ExpenseAlreadyDeleted = "DOM2003";
    public const string InvalidPurchaseDateEdit = "DOM2004";
    public const string InvalidExpenseDescription = "DOM2005";
    public const string InvalidAccountId = "DOM2006";
}
