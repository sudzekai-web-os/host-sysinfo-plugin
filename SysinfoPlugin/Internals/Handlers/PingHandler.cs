using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Core;
using System.Net;

namespace SysinfoPlugin
{
    internal sealed class PingHandler : SysinfoRequestHandler
    {
        public override string Pattern => "sysinfo/ping";

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Проверка связи с сервером", ["sysinfo"], "Проверка связи с сервером")
        {
            Description = "Отвечает без тела для измерения задержки HTTP-запроса на клиенте.",
            Responses = [
                new(204, description: "Сервер ответил", contentType: ResponseContentType.None),
                new(401, description: "Требуется действующий токен"),
                new(403, description: "Недостаточно прав")
            ]
        };

        protected override HttpResponseDto Execute()
            => HttpResponseDto.Create(HttpStatusCode.NoContent);
    }
}
