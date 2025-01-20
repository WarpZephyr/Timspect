using System.IO;

namespace Timspect.Unpacker.Helpers
{
    internal static class FileHelper
    {
        public static void Backup(string path)
        {
            if (File.Exists(path))
            {
                string backupPath = path + ".bak";
                if (!File.Exists(backupPath))
                {
                    File.Move(path, backupPath);
                }
            }
        }
    }
}
