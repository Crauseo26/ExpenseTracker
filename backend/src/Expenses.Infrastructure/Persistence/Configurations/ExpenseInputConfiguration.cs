using Expenses.Domain.Aggregates.ExpenseInput;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expenses.Infrastructure.Persistence.Configurations;

public class ExpenseInputConfiguration : IEntityTypeConfiguration<ExpenseInput>
{
    public void Configure(EntityTypeBuilder<ExpenseInput> builder)
    {
        builder.ToTable("ExpenseInputs");

        builder.HasKey(ei => ei.Id);

        builder.Property(ei => ei.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(ei => ei.UserId)
            .IsRequired();

        builder.Property(ei => ei.InputType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ei => ei.RawContent)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(ei => ei.NormalizedContent)
            .IsRequired(false)
            .HasColumnType("nvarchar(max)");

        builder.Property(ei => ei.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ei => ei.ErrorMessage)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(ei => ei.CreatedAt)
            .IsRequired();

        builder.Property(ei => ei.ProcessedAt)
            .IsRequired(false);

        builder.Property(ei => ei.DeletedAt)
            .IsRequired(false);

        builder.HasIndex(ei => ei.UserId);
        builder.HasIndex(ei => ei.CreatedAt);

        builder.HasQueryFilter(ei => ei.DeletedAt == null);
    }
}
