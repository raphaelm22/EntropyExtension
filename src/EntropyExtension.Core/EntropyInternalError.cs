using System;
using System.IO;
using System.Text;

namespace EntropyExtension.Core
{
    internal class EntropyInternalError
    {
        private static readonly object _fileLockObject = new object();

        internal static void Save(Exception ex)
        {
            var log = new StringBuilder();

            log.AppendLine($"Error time: {DateTime.Now}");

            var exLog = ex;
            while (exLog != null)
            {
                log.AppendLine($"{ex.GetType().FullName}: {ex.Message}");
                if (ex.StackTrace != null)
                    log.AppendLine($"StackTrace: {ex.StackTrace}");

                ex = ex.InnerException;
            }
            log.AppendLine();

            lock (_fileLockObject)
            {
                File.AppendAllText(FilePath(), log.ToString());
            }
        }

        internal static string FilePath()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "bin");
            if (!Directory.Exists(path))
                path = Directory.GetCurrentDirectory();

            return Path.Combine(path, "EntropyExtension.log");
        }
    }
}
