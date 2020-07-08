using KSBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSGlobal
{
    public static class App
    {
        public static string Path => System.Environment.CurrentDirectory;
        public static string AppDataPath => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Company);
        public static string TempPath => System.IO.Path.Combine(System.IO.Path.GetTempPath(), Company);
        public static string Company => Config.Get("Company").ToString();
        public static string Namespace => Config.Get("Namespace").ToString();
        public static string MasterKey => Config.Get("MasterKey").ToString();
        public static string Language => Config is null ? "en-gb" : Config.Language; 
        public static KSInterfaces.IConfig Config { get; internal set; }

        public static void AddConfig<T>(T config) where T : KSInterfaces.IConfig
        {
            if (Config is null)
                Config = config;
            else
                Config.Join(config);
        }

        public static void SetConfig<T>(T config) where T : KSInterfaces.IConfig
        {
            if (Config is null)
                Config = config;
            else
                Config.Join(config);
        }
    }
}
