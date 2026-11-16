using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Core;
using System.Net;

namespace SysinfoPlugin
{
    internal abstract class SysinfoRequestHandler : IRequestHandler
    {
        public abstract string Pattern { get; }

        public HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public abstract HandlerMetadata Metadata { get; }

        public HttpResponseDto Handle(HttpRequestDto request)
        {
            if (request.Claims.GetValueOrDefault("sub") is not [var subject] || !long.TryParse(subject, out var id) || id <= 0)
                return HttpResponseDto.CreateError(HttpStatusCode.Unauthorized, 401, "Требуется действующий токен.");

            var roles = request.Claims.GetValueOrDefault("role") ?? [];
            if (!roles.Any(role => role is nameof(ClientRole.Viewer) or nameof(ClientRole.User) or nameof(ClientRole.Administrator)))
                return HttpResponseDto.CreateError(HttpStatusCode.Forbidden, 403, "Недостаточно прав.");

            return Execute();
        }

        protected abstract HttpResponseDto Execute();
    }
}
