using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Core;
using SysinfoPlugin.Application.Dispatchering.Commands;
using SysinfoPlugin.Application.Dispatchering.Dispatchers;
using SysinfoPlugin.Application.Dto;
using System.Net;

namespace SysinfoPlugin
{
    internal sealed class GetSystemInfoHandler(IDispatcher dispatcher) : SysinfoRequestHandler
    {
        public override string Pattern => "sysinfo/info";

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Информация о сервере", ["sysinfo"], "Информация о сервере")
        {
            Description = "Возвращает имя и ОС сервера, архитектуру, время работы, сведения о процессоре, ОЗУ, локальных дисках и видеопроцессорах.",
            Responses = [
                new(200, LinCtrlContext.GetTypeInfo<SystemSnapshot>(), "Сведения о сервере"),
                new(401, description: "Требуется действующий токен"),
                new(403, description: "Недостаточно прав"),
                new(500, description: "Не удалось получить сведения о сервере")
            ]
        };

        protected override HttpResponseDto Execute()
            => HttpResponseDto.Create(HttpStatusCode.OK).AsJson(dispatcher.Execute<GetSystemInfoCommand, SystemSnapshot>(new()));
    }
}
