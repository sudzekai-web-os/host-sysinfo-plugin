using SysinfoPlugin.Application.Dto;

namespace SysinfoPlugin.Application.Services.SystemInformation
{
    internal sealed class StorageService
    {
        public StorageVolume[] GetVolumes()
        {
            var volumes = new List<StorageVolume>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    var type = drive.DriveType;
                    if (type != DriveType.Fixed && !(OperatingSystem.IsLinux() && drive.Name == "/" && type == DriveType.Unknown))
                        continue;

                    if (!drive.IsReady || !Directory.Exists(drive.RootDirectory.FullName))
                        continue;

                    var total = drive.TotalSize;
                    if (total > 0)
                        volumes.Add(new(drive.Name, new(total, Math.Clamp(drive.TotalFreeSpace, 0, total))));
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    // A volume can be unmounted while its capacity is being read.
                }
            }

            return [.. volumes.OrderBy(volume => volume.Name, StringComparer.OrdinalIgnoreCase)];
        }
    }
}
