using BLL.BusinessLogicLayer.Services.Auth;
using BLL.BusinessLogicLayer.Services.Email;
using BLL.BusinessLogicLayer.Services.Scheduling;
using BLL.BusinessLogicLayer.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.BusinessLogicLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        // Add UnitOfWork as Singleton or Scoped (currently it's a Singleton in code, let's keep it simple for now or convert it)
        services.AddSingleton<UnitOfWork>(UnitOfWork.Instance);

        // Add Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        
        // Add more services here as they are implemented
        // services.AddScoped<IExportService, ExportService>();
        // services.AddScoped<IImportService, ImportService>();

        return services;
    }
}
