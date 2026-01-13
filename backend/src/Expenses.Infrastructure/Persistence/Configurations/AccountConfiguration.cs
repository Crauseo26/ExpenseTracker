using Expenses.Domain.Aggregates.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expenses.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.ExpenseGroupId)
            .IsRequired();

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.DeletedAt)
            .IsRequired(false);

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.ExpenseGroupId);

        builder.HasQueryFilter(a => a.DeletedAt == null);
    }
}
