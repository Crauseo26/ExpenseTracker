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

    // Account errors: 3xxx
    public const string InvalidAccountName = "DOM3001";
    public const string AccountAlreadyDeleted = "DOM3002";
    public const string InvalidExpenseGroupId = "DOM3003";

    // ExpenseGroup errors: 4xxx
    public const string InvalidExpenseGroupName = "DOM4001";
    public const string ExpenseGroupAlreadyDeleted = "DOM4002";

    // ExpenseInput errors: 5xxx
    public static class ExpenseInput
    {
        public const string InvalidUserId = "DOM5001";
        public const string InvalidRawContent = "DOM5002";
        public const string RawContentTooLong = "DOM5003";
        public const string InvalidNormalizedContent = "DOM5004";
        public const string NormalizedContentTooLong = "DOM5005";
        public const string CannotModifyProcessedInput = "DOM5006";
        public const string AlreadyProcessed = "DOM5007";
        public const string CannotProcessErrorInput = "DOM5008";
        public const string CannotProcessDeletedInput = "DOM5009";
        public const string CannotMarkProcessedAsError = "DOM5010";
        public const string InvalidErrorMessage = "DOM5011";
        public const string CannotMarkDeletedAsError = "DOM5012";
        public const string AlreadyDeleted = "DOM5013";
    }
}
