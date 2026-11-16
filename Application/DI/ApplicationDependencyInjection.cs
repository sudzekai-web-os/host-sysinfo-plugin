using Microsoft.Extensions.DependencyInjection;
using SysinfoPlugin.Application.Dispatchering.Commands;
using SysinfoPlugin.Application.Dispatchering.Dispatchers;
using SysinfoPlugin.Application.Dispatchering.Handlers;
using SysinfoPlugin.Application.Dto;
using SysinfoPlugin.Application.Services.SystemInformation;

namespace SysinfoPlugin.Application.DI
{
    public static class ApplicationDependencyInjection
    {
        extension(IServiceCollection services)
        {
            public void AddApplicationServices()
            {
                services.AddSingleton<StorageService>();
                services.AddSingleton<SystemInformationService>();
                services.AddSingleton<IDispatcher, Dispatcher>();

                services.AddTransient<ICommandHandler<GetSystemInfoCommand, SystemSnapshot>, GetSystemInfoCommandHandler>();
            }
        }
    }
}
