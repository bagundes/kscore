using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KSShell
{
    public static class Directory
    {
        private static string _Path => KSGlobal.App.Path;
        private static string _TempPath => KSGlobal.App.TempPath;
        private static string _DataPath => KSGlobal.App.AppDataPath;


        public static string TempDataFolder(params string[] folders) => CreateFolder(_TempPath, folders);

        public static string AppDataFolder(params string[] folders) => CreateFolder(_DataPath, folders);

        private static string CreateFolder(string root, params string[] folders)
        {
            string specificFolder = String.Empty;

            try
            {
                folders = folders ?? new string[0];
                var path = new string[folders.Length + 1];

                path[0] = root;
                folders.CopyTo(path, 1);

                specificFolder = Path.Combine(path);

                if (!System.IO.Directory.Exists(specificFolder))
                    System.IO.Directory.CreateDirectory(specificFolder);

                return specificFolder;
            }
            catch (Exception ex)
            {
                throw new KS.KSException(ex, KS.Enums.MessageCode.CreateFolder_1, specificFolder);
            }
        }
    }
}
