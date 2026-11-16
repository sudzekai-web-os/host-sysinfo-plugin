namespace SysinfoPlugin.Application.Services.SystemInformation
{
    internal static class LinuxGraphicsAdapters
    {
        private static readonly string[] _pciIdsPaths = ["/usr/share/hwdata/pci.ids", "/usr/share/misc/pci.ids", "/usr/share/pci.ids"];

        public static string[] GetNames()
        {
            string[] devices;
            try
            {
                devices = Directory.GetDirectories("/sys/class/drm", "card*");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                return [];
            }

            var names = new List<string>();
            foreach (var card in devices.Order(StringComparer.Ordinal))
            {
                var name = Path.GetFileName(card);
                if (name.Length <= 4 || !name.AsSpan(4).ContainsOnlyAsciiDigits())
                    continue;

                var device = Path.Combine(card, "device");
                var product = LinuxSystemFile.ReadLines(Path.Combine(device, "product_name")).FirstOrDefault();
                var events = LinuxSystemFile.ReadLines(Path.Combine(device, "uevent"));
                var driver = LinuxSystemFile.GetValue(events, "DRIVER", '=');
                var compatible = LinuxSystemFile.GetValue(events, "OF_COMPATIBLE_0", '=');
                var pciId = LinuxSystemFile.GetValue(events, "PCI_ID", '=');
                var adapter = product ?? ReadPciName(pciId) ?? compatible ?? driver;
                if (!string.IsNullOrWhiteSpace(adapter))
                    names.Add(adapter == driver && pciId != null ? $"{adapter} ({pciId})" : adapter);
                else if (pciId != null)
                    names.Add($"PCI {pciId}");
            }

            return [.. names];
        }

        private static string? ReadPciName(string? id)
        {
            if (id == null || id.Length != 9 || id[4] != ':')
                return null;

            var vendor = id.AsSpan(0, 4);
            var device = id.AsSpan(5, 4);
            foreach (var path in _pciIdsPaths)
            {
                if (!File.Exists(path))
                    continue;

                try
                {
                    var vendorFound = false;
                    foreach (var line in File.ReadLines(path))
                    {
                        if (line.Length < 6 || line[0] == '#')
                            continue;

                        if (line[0] != '\t')
                        {
                            if (vendorFound)
                                break;
                            vendorFound = line.AsSpan(0, 4).Equals(vendor, StringComparison.OrdinalIgnoreCase);
                        }
                        else if (vendorFound && line[1] != '\t' && line.AsSpan(1, 4).Equals(device, StringComparison.OrdinalIgnoreCase))
                        {
                            return line[5..].Trim();
                        }
                    }
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    continue;
                }
            }

            return null;
        }

        private static bool ContainsOnlyAsciiDigits(this ReadOnlySpan<char> value)
        {
            foreach (var character in value)
                if (!char.IsAsciiDigit(character))
                    return false;
            return true;
        }
    }
}
