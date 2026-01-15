using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Expenses.Domain.Aggregates.User;
using Expenses.Domain.Interfaces;
using Expenses.Infrastructure.Persistence;
using Expenses.Infrastructure.Repositories;
using Expenses.Application.Interfaces;
using Expenses.Infrastructure.Services.AI;

namespace Expenses.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<AppDbContext>(options =>
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseInMemoryDatabase("ExpensesInMemoryDb");
            }
            else
            {
                options.UseSqlServer(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            }
        });

        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            
            options.User.RequireUniqueEmail = true;
            
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IExpenseGroupRepository, ExpenseGroupRepository>();
        services.AddScoped<IExpenseInputRepository, ExpenseInputRepository>();

        services.AddHttpClient<IAIOrchestrationService, AIOrchestrationService>();

        return services;
    }
}
