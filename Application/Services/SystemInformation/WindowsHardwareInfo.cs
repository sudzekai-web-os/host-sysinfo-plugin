using Microsoft.Win32;
using SysinfoPlugin.Application.Dto;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace SysinfoPlugin.Application.Services.SystemInformation
{
    [SupportedOSPlatform("windows")]
    internal static partial class WindowsHardwareInfo
    {
        public static string? GetProcessorName()
        {
            try
            {
                using var processor = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                return (processor?.GetValue("ProcessorNameString") as string)?.Trim();
            }
            catch (Exception ex) when (ex is IOException or SecurityException or UnauthorizedAccessException)
            {
                return null;
            }
        }

        public static CapacityInfo? GetMemory()
        {
            var status = new MemoryStatus { Length = (uint)Marshal.SizeOf<MemoryStatus>() };
            if (GlobalMemoryStatusEx(ref status) == 0 || status.TotalPhysical == 0)
                return null;

            return new((long)status.TotalPhysical, (long)Math.Min(status.AvailablePhysical, status.TotalPhysical));
        }

        [LibraryImport("kernel32.dll")]
        private static partial int GlobalMemoryStatusEx(ref MemoryStatus status);

        [StructLayout(LayoutKind.Sequential)]
        private struct MemoryStatus
        {
            public uint Length;
            public uint Load;
            public ulong TotalPhysical;
            public ulong AvailablePhysical;
            public ulong TotalPageFile;
            public ulong AvailablePageFile;
            public ulong TotalVirtual;
            public ulong AvailableVirtual;
            public ulong AvailableExtendedVirtual;
        }
    }
}
