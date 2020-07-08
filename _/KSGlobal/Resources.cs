using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KSGlobal
{
    public static class Resources
    {
        public static string[] GetAllResources(Assembly assembly, string name)
        {
            try
            {
                var list = assembly.GetManifestResourceNames();
                return list.Where(t => t.EndsWith($"{name}", StringComparison.InvariantCultureIgnoreCase)).ToArray();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Create a file from Resource
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name">File name</param>
        /// <param name="replace">Replace file on cache</param>
        /// <returns></returns>
        public static FileInfo GetFile(Assembly assembly, string name, bool replace = false)
        {
            try
            {
                var path = System.IO.Path.Combine(KSGlobal.App.Path, name);

                if (!replace && File.Exists(path))
                    return new FileInfo(path);

                var resx = GetAllResources(assembly, name).FirstOrDefault();
                if (String.IsNullOrEmpty(resx)) return null;

                using (var stream = assembly.GetManifestResourceStream(resx))
                {
                    using (Stream s = File.Create(path))
                    {
                        stream.CopyTo(s);
                    }
                }

                return new FileInfo(path);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static string GetString(Assembly assembly, string name)
        {
            var resx = GetAllResources(assembly, name).FirstOrDefault();
            if (String.IsNullOrEmpty(resx)) return null;

            using (var stream = assembly.GetManifestResourceStream(resx))
            {
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Read the resource as dictionary
        /// </summary>
        /// <param name="assembly">assembly running</param>
        /// <param name="name">resource name</param>
        /// <returns>Dictionary of values</returns>
        public static Dictionary<string, object> GetDictionay(Assembly assembly, string name)
        {
            var resx = GetAllResources(assembly, name).ToArray();
            if (resx == null || resx.Length < 1) return null;

            var res = new Dictionary<string, object>();
            foreach (var foo in resx)
                using (var stream = assembly.GetManifestResourceStream(foo))
                using (var sr = new StreamReader(stream))
                    foreach (var foobar in JsonConvert.DeserializeObject<Dictionary<string, object>>(sr.ReadToEnd()))
                        if (!res.ContainsKey(foobar.Key))
                            res.Add(foobar.Key, foobar.Value);
            return res;
        }
    }
}
