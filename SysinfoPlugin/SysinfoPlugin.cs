using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Assembly;
using Sudzekai.LinCtrl.Core;
using SysinfoPlugin.Application.DI;

namespace SysinfoPlugin
{
    [PluginAssembly]
    public partial class SysinfoPlugin : IPluginAssembly
    {
        public void Configure(AssemblyHostBuilder builder)
        {
            LinCtrlContext.AddTypeInfoResolver(SysinfoJsonContext.Default);

            builder.Services.AddApplicationServices();
            builder.Services.AddTransient<IRequestHandler, GetSystemInfoHandler>();
            builder.Services.AddTransient<IRequestHandler, PingHandler>();
        }
    }
}
