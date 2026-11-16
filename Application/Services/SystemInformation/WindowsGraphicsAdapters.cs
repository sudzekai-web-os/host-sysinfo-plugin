using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace SysinfoPlugin.Application.Services.SystemInformation
{
    [SupportedOSPlatform("windows")]
    internal static unsafe partial class WindowsGraphicsAdapters
    {
        private const int _releaseSlot = 2;
        private const int _enumAdapters1Slot = 12;
        private const int _getDesc1Slot = 10;
        private const uint _softwareAdapter = 2;
        private static readonly Guid _factoryId = new("770aae78-f26f-4dba-a829-253c83d1b387");

        public static string[] GetNames()
        {
            nint factory;
            try
            {
                if (CreateDXGIFactory1(in _factoryId, out factory) < 0 || factory == 0)
                    return [];
            }
            catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
            {
                return [];
            }

            try
            {
                var enumerate = (delegate* unmanaged[Stdcall]<nint, uint, nint*, int>)(*(nint**)factory)[_enumAdapters1Slot];
                var names = new List<string>();
                for (uint index = 0; ; index++)
                {
                    nint adapter;
                    if (enumerate(factory, index, &adapter) < 0 || adapter == 0)
                        break;

                    try
                    {
                        if (GetName(adapter) is { } name)
                            names.Add(name);
                    }
                    finally
                    {
                        Release(adapter);
                    }
                }
                return [.. names];
            }
            finally
            {
                Release(factory);
            }
        }

        private static string? GetName(nint adapter)
        {
            var getDescription = (delegate* unmanaged[Stdcall]<nint, AdapterDescription*, int>)(*(nint**)adapter)[_getDesc1Slot];
            AdapterDescription description;
            if (getDescription(adapter, &description) < 0 || (description.Flags & _softwareAdapter) != 0)
                return null;

            var buffer = new ReadOnlySpan<char>(description.Name, 128);
            var end = buffer.IndexOf('\0');
            var name = (end < 0 ? buffer : buffer[..end]).Trim();
            return name.IsEmpty ? null : name.ToString();
        }

        private static void Release(nint instance)
            => ((delegate* unmanaged[Stdcall]<nint, uint>)(*(nint**)instance)[_releaseSlot])(instance);

        [LibraryImport("dxgi.dll")]
        private static partial int CreateDXGIFactory1(in Guid interfaceId, out nint factory);

        [StructLayout(LayoutKind.Sequential)]
        private struct AdapterDescription
        {
            public fixed char Name[128];
            public uint VendorId;
            public uint DeviceId;
            public uint SubSysId;
            public uint Revision;
            public nuint DedicatedVideoMemory;
            public nuint DedicatedSystemMemory;
            public nuint SharedSystemMemory;
            public uint LuidLowPart;
            public int LuidHighPart;
            public uint Flags;
        }
    }
}
