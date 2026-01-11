using Expenses.Domain.Exceptions;

namespace Expenses.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new DomainException(ErrorCodes.InvalidMoneyAmount, "Amount cannot be negative");

        if (decimal.Round(amount, 2) != amount)
            throw new DomainException(ErrorCodes.InvalidMoneyAmount, "Amount cannot have more than 2 decimal places");

        Amount = amount;
        Currency = currency;
    }
}
