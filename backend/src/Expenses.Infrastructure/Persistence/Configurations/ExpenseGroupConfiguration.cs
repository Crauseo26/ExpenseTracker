using Expenses.Domain.Aggregates.ExpenseGroup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expenses.Infrastructure.Persistence.Configurations;

public class ExpenseGroupConfiguration : IEntityTypeConfiguration<ExpenseGroup>
{
    public void Configure(EntityTypeBuilder<ExpenseGroup> builder)
    {
        builder.ToTable("ExpenseGroups");

        builder.HasKey(eg => eg.Id);

        builder.Property(eg => eg.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(eg => eg.UserId)
            .IsRequired();

        builder.Property(eg => eg.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(eg => eg.CreatedAt)
            .IsRequired();

        builder.Property(eg => eg.DeletedAt)
            .IsRequired(false);

        builder.HasIndex(eg => eg.UserId);

        builder.HasQueryFilter(eg => eg.DeletedAt == null);
    }
}
