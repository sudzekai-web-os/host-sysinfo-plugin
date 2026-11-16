using SysinfoPlugin.Application.Dto;
using System.Globalization;

namespace SysinfoPlugin.Application.Services.SystemInformation
{
    internal static class LinuxHardwareInfo
    {
        public static string? GetOperatingSystem()
        {
            var lines = LinuxSystemFile.ReadLines("/etc/os-release");
            if (lines.Length == 0)
                lines = LinuxSystemFile.ReadLines("/usr/lib/os-release");

            return (LinuxSystemFile.GetValue(lines, "PRETTY_NAME", '=') ?? LinuxSystemFile.GetValue(lines, "NAME", '='))
                ?.Trim('"', '\'');
        }

        public static string? GetProcessorName()
        {
            var lines = LinuxSystemFile.ReadLines("/proc/cpuinfo");
            return LinuxSystemFile.GetValue(lines, "model name", ':')
                ?? LinuxSystemFile.GetValue(lines, "Processor", ':')
                ?? LinuxSystemFile.GetValue(lines, "Hardware", ':');
        }

        public static CapacityInfo? GetMemory()
        {
            var lines = LinuxSystemFile.ReadLines("/proc/meminfo");
            var total = ReadKilobytes(lines, "MemTotal");
            var available = ReadKilobytes(lines, "MemAvailable") ?? ReadKilobytes(lines, "MemFree");
            if (total is null or <= 0 || available == null)
                return null;

            return new(total.Value * 1024, Math.Min(available.Value, total.Value) * 1024);
        }

        private static long? ReadKilobytes(string[] lines, string key)
        {
            var value = LinuxSystemFile.GetValue(lines, key, ':');
            if (value == null || !value.EndsWith("kB", StringComparison.Ordinal))
                return null;

            if (!long.TryParse(value.AsSpan(0, value.Length - 2).Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var kilobytes)
                || kilobytes > long.MaxValue / 1024)
                return null;

            return kilobytes;
        }
    }
}
