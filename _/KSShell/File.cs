using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Linq;
using KS;

namespace KSShell
{
    public static class File
    {
        #region Write file
        public static string Write(string text, string name, string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            var file = Path.Combine(path, name);
            System.IO.File.WriteAllText(file, text);
            return file;
        }

        #endregion
        #region Create file
        /// <summary>
        /// Create a temp file
        /// </summary>
        /// <param name="ext">Extension</param>
        /// <param name="name">File name</param>
        /// <param name="create">Create a file?</param>
        /// <returns>Path the temp file</returns>
        public static string TempFile(string ext = "tmp", string name = null, bool create = true)
        {
            var file = name ?? $"{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            file += $".{ext}";
            var dir = KSShell.Directory.TempDataFolder("touch");
            file = Path.Combine(dir, file);
            if (create) CreateFile(new MemoryStream(1), file, true);
            return file;
        }
        public static FileInfo CreateFile(byte[] bytes, string path, bool replace = false)
        {
            if (bytes == null || bytes.Length < 1)
                throw new Exception("The bytes cannot be empty");
            return CreateFile((Stream)new MemoryStream(bytes), path, replace);
        }
        public static FileInfo CreateFile(Stream stream, string path, bool replace = false)
        {
            if (stream == null)
                throw new Exception("The stream is not to be empty");
            if (System.IO.File.Exists(path) && !replace)
                return new FileInfo(path);
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                stream.CopyTo((Stream)fileStream);
                stream.Close();
            }
            return new FileInfo(path);
        }
        public static FileInfo CreateFile(string contents, string path, bool replace = false)
        {
            if (System.IO.File.Exists(path) && !replace)
                return new FileInfo(path);

            System.IO.File.WriteAllText(path, contents);

            return new FileInfo(path);
        }
        #endregion
        #region Load file
        public static string Load(string name, string path)
        {
            return Load(Path.Combine(path, name));

        }

        public static string Load(FileInfo fileInfo)
        {
            return Load(fileInfo.FullName);
        }

        public static string Load(string fullName)
        {
            return System.IO.File.ReadAllText(fullName);
        }
        #endregion
        #region Save  file
        public static void Save(string[] lines, string filename, bool ovride = false)
        {
            var tried = 0;
            var fileInfo = new FileInfo(filename);
            System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);

            var exists = System.IO.File.Exists(fileInfo.FullName);

            while (IsLocked(fileInfo.FullName) && exists)
            {
                System.Threading.Thread.Sleep(2000);

                //TODO: Fix bug when log/track file is being used
                if (tried++ > 20)
                {
                    Save($"The {fileInfo.Name} file is locked a long time.", fileInfo.FullName, false);
                    fileInfo = new FileInfo(fileInfo.FullName + "_locked");
                }
            }

            if (ovride)
            {
                if (WaitUnlocked(fileInfo.FullName))
                    System.IO.File.WriteAllLines(fileInfo.FullName, lines);
            }
            else
            {
                using (var file = new System.IO.StreamWriter(fileInfo.FullName, true))
                {
                    for (int i = 0; i < lines.Length; i++)
                        file.WriteLine(lines[i]);
                }
            }
        }

        public static void Save(string line, string filename, bool ovride = false)
        {
            Save(new string[] { line }, filename, ovride);
        }

        public static string Save(Stream stream, string filename, bool ovride = true)
        {
            var fileInfo = new FileInfo(filename);

            if (stream == null)
                throw new Exception("The stream is not to be empty");


            if (fileInfo.Exists && !ovride)
                return fileInfo.FullName;

            using (var output = new FileStream(fileInfo.FullName, FileMode.OpenOrCreate))
            {
                stream.CopyTo(output);
                stream.Close();
            }

            return fileInfo.FullName;
        }
        #endregion
        #region Status
        public static bool WaitUnlocked(string filename)
        {
            while (IsLocked(filename))
                System.Threading.Thread.Sleep(1000);

            return true;
        }
        public static bool IsLocked(string file)
        {
            return IsLocked(new FileInfo(file));
        }

        public static bool IsLocked(FileInfo file)
        {
            file.Refresh();
            FileStream stream = null;

            try
            {
                if (file.Exists)
                    stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);

                //arquivo está disponível
                return false;
            }
            catch (IOException)
            {
                //o arquivo está indisponível pelas seguintes causas:
                //está sendo escrito
                //utilizado por uma outra thread
                //não existe ou sendo criado
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
        #endregion
        #region Compact files
        public static void Compress(string[] files, string destination)
        {
            throw new NotImplementedException();
        }
        public static void ZipFiles(FileInfo[] files, string destination)
        {
            ZipFiles(Array.ConvertAll(files, t => t.FullName), destination);
        }

        public static void ZipFiles(string[] files, string destination)
        {
            //using (var zip = new ZipFile())
            //{
            //    foreach (var file in files)
            //    {
            //        if (System.IO.File.Exists(file))
            //            zip.AddFile(file);

            //        zip.Save(destination);
            //    }

            //    if (Global.App.Debug && files.Length > 0)
            //    {
            //        var track = Helpers.DiagnosticHelper.TrackMessages(files);
            //        Helpers.DiagnosticHelper.Debug(typeof(File).Name, track, "Compacted {0} files to {1}", files.Length, destination);
            //    }
            //}
        }
        #endregion
        #region Delete
        public static void Delete(string file)
        {
            System.IO.File.Delete(file);
        }

        public static void Delete(string[] files)
        {
            foreach (var file in files)
                System.IO.File.Delete(file);
        }
        #endregion
        #region Find
        public static bool TryFind(out FileInfo[] files, string path, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories, DateTime? lastAccess = null)
        {
            files = Find(path, searchPattern, searchOption, lastAccess);
            return files.Length > 0;
        }

        public static FileInfo[] Find(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories, DateTime? lastAccess = null)
        {
            var access = lastAccess ?? DateTime.Now;
            try
            {
                var files = System.IO.Directory.GetFiles(path, searchPattern, searchOption)
                    .Select(f => new FileInfo(f))
                    .Where(f => f.LastAccessTime < access)
                    .ToList()
                    .ToArray();

                return files;

            }
            catch (Exception ex)
            {
                throw new KSException(ex, KS.Enums.MessageCode.FindFiles_0);
            }
        }
        #endregion
        #region Convert
        public static string FileToBase64(string file)
        {
            try
            {
                var foo = new FileInfo(file);
                byte[] binData = System.IO.File.ReadAllBytes(file);
                return $"[{foo.Extension}]{Convert.ToBase64String(binData)}";
            }
            catch
            {
                return String.Empty;
            }

        }

        public static System.Drawing.Image Base64ToImage(string base64String)
        {
            base64String = base64String.Replace("data:image/png;base64,", "");
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                var image = System.Drawing.Image.FromStream(ms, true);
                return image;
            }
        }
        /// <summary>
        /// Convert base 64 to file
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static string Base64ToFile(string base64)
        {
            var foo = Convert.FromBase64String(base64);
            throw new NotImplementedException();
        }
        
        #endregion
    }
}
