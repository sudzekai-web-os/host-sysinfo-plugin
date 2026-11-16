namespace SysinfoPlugin.Application.Services.SystemInformation
{
    internal static class LinuxSystemFile
    {
        public static string[] ReadLines(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                return [];
            }
        }

        public static string? GetValue(IEnumerable<string> lines, string key, char separator)
        {
            foreach (var line in lines)
            {
                var split = line.IndexOf(separator);
                if (split >= 0 && line.AsSpan(0, split).Trim().SequenceEqual(key))
                    return line[(split + 1)..].Trim();
            }

            return null;
        }
    }
}
