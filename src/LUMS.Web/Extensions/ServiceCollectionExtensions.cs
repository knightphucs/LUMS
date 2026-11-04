using LUMS.Core.Abstractions;
using LUMS.Core.Repositories;
using LUMS.Core.Services;
using LUMS.Core.Services.Interfaces;
using LUMS.Infrastructure.Data;
using LUMS.Infrastructure.Repositories;
using LUMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace LUMS.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLumsApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LumsDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Chưa cấu hình ConnectionStrings:LumsDb. Xem README (mục 'Chạy dự án') để cấu hình SQL Server.");
        }

        services.AddDbContext<LumsDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IFineRepository, FineRepository>();
        services.AddScoped<ISystemLogRepository, SystemLogRepository>();

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();

        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IPolicyService, PolicyService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
