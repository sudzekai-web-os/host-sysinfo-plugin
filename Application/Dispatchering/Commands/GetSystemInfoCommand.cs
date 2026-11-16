using SysinfoPlugin.Application.Dto;

namespace SysinfoPlugin.Application.Dispatchering.Commands
{
    public sealed record GetSystemInfoCommand : ICommand<SystemSnapshot>;
}
