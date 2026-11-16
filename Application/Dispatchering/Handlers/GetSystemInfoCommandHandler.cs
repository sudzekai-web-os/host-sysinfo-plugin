using SysinfoPlugin.Application.Dispatchering.Commands;
using SysinfoPlugin.Application.Dto;
using SysinfoPlugin.Application.Services.SystemInformation;

namespace SysinfoPlugin.Application.Dispatchering.Handlers
{
    internal sealed class GetSystemInfoCommandHandler(SystemInformationService information)
        : ICommandHandler<GetSystemInfoCommand, SystemSnapshot>
    {
        public SystemSnapshot Handle(GetSystemInfoCommand command)
            => information.GetSnapshot();
    }
}
