namespace SysinfoPlugin.Application.Dto
{
    public sealed record SystemSnapshot(
        string HostName,
        string OperatingSystem,
        string Architecture,
        long UptimeSeconds,
        ProcessorInfo Processor,
        CapacityInfo? Memory,
        StorageVolume[] Storage,
        string[] GraphicsAdapters);
}
