using SysinfoPlugin.Application.Dto;
using System.Text.Json.Serialization;

namespace SysinfoPlugin
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(SystemSnapshot))]
    internal partial class SysinfoJsonContext : JsonSerializerContext
    {
    }
}
