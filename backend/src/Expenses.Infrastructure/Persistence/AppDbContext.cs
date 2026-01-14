using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Expenses.Domain.Aggregates.User;
using Expenses.Infrastructure.Persistence.Configurations;

namespace Expenses.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(u => u.CreatedAt).HasColumnName("created_at");
            entity.Property(u => u.DeletedAt).HasColumnName("deleted_at");
            entity.HasQueryFilter(u => u.DeletedAt == null);
        });

        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");

        modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new ExpenseGroupConfiguration());
        modelBuilder.ApplyConfiguration(new ExpenseInputConfiguration());
    }
}
