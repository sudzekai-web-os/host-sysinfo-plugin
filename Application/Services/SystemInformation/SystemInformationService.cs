using SysinfoPlugin.Application.Dto;
using System.Runtime.InteropServices;

namespace SysinfoPlugin.Application.Services.SystemInformation
{
    internal sealed class SystemInformationService(StorageService storage)
    {
        public SystemSnapshot GetSnapshot()
        {
            var operatingSystem = RuntimeInformation.OSDescription;
            string? processor = null;
            CapacityInfo? memory = null;
            string[] graphicsAdapters = [];

            if (OperatingSystem.IsWindows())
            {
                processor = WindowsHardwareInfo.GetProcessorName();
                memory = WindowsHardwareInfo.GetMemory();
                graphicsAdapters = WindowsGraphicsAdapters.GetNames();
            }
            else if (OperatingSystem.IsLinux())
            {
                operatingSystem = LinuxHardwareInfo.GetOperatingSystem() ?? operatingSystem;
                processor = LinuxHardwareInfo.GetProcessorName();
                memory = LinuxHardwareInfo.GetMemory();
                graphicsAdapters = LinuxGraphicsAdapters.GetNames();
            }

            return new(
                Environment.MachineName,
                operatingSystem,
                RuntimeInformation.OSArchitecture.ToString(),
                Environment.TickCount64 / 1000,
                new(processor, Environment.ProcessorCount),
                memory,
                storage.GetVolumes(),
                graphicsAdapters);
        }
    }
}
