using Expenses.Application.Commands;
using Expenses.Application.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Expenses.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateExpenseCommandHandler>();
        services.AddScoped<UpdateExpenseCommandHandler>();
        services.AddScoped<ConfirmExpenseCommandHandler>();
        services.AddScoped<DeleteExpenseCommandHandler>();
        services.AddScoped<GetExpenseByIdQueryHandler>();
        services.AddScoped<GetExpensesByUserQueryHandler>();

        services.AddScoped<CreateAccountGroupCommandHandler>();
        services.AddScoped<UpdateAccountGroupCommandHandler>();
        services.AddScoped<DeleteAccountGroupCommandHandler>();
        services.AddScoped<GetAccountGroupByIdQueryHandler>();
        services.AddScoped<GetAccountGroupsByUserQueryHandler>();

        services.AddScoped<CreateAccountCommandHandler>();
        services.AddScoped<UpdateAccountCommandHandler>();
        services.AddScoped<DeleteAccountCommandHandler>();
        services.AddScoped<GetAccountByIdQueryHandler>();
        services.AddScoped<GetAccountsByUserQueryHandler>();

        return services;
    }
}
