using SysinfoPlugin.Application.Dispatchering.Commands;

namespace SysinfoPlugin.Application.Dispatchering.Dispatchers
{
    public interface IDispatcher
    {
        TResult Execute<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
    }
}
