using LUMS.Core.Interfaces.Services;
using LUMS.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LUMS.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IPolicyService, PolicyService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
