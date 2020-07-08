using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KSHelpers
{
    /// <summary>
    /// Local file Helper
    /// </summary>
    public class LocalHelper
    {
        public static List<KSBase.Message> LoadMessages(Assembly assembly)
        {
            var messageModel = new List<KSBase.Message>();
            var messages = ResourceHelper.GetDictionay(assembly, $"messages_{KSGlobal.App.Language}.json");
            var messages_link = ResourceHelper.GetDictionay(assembly, $"messages_link.json");

            foreach (var message in messages)
            {
                var number = int.Parse(KS.Dynamic.From(message.Key).GetOnlyNumbers());
                var link = messages_link.Where(t => t.Key.Equals(message.Key)).Select(t => t.Value).FirstOrDefault();
                messageModel.Add(new KSBase.Message(number, message.Value.ToString(), link));
            }

            return messageModel;
        }

        public static void LoadConfig<T>(Assembly assembly, ref T localConfig) where T : KSInterfaces.IConfig, new()
        {
            #region loading config
            var json = KSHelpers.ResourceHelper.GetString(assembly, "config.json");
            
            // If config is not embedded, method will get in the folder project 
            if (string.IsNullOrEmpty(json))
            {
                var path = Environment.CurrentDirectory;
                var paths = KSShell.File.Find(path, "config.json");
                if(paths.Length > 0)
                json = KSShell.File.Load(paths[0]);
            }

            if(!String.IsNullOrEmpty(json))
                localConfig.Load(json);
            #endregion

            #region loading settings
            if (KSShell.File.TryFind(out System.IO.FileInfo[] files, KSGlobal.App.Path, "settings.json", System.IO.SearchOption.TopDirectoryOnly))
            {
                var file = files[0];
                json = KSShell.File.Load(file.FullName);

            }

            localConfig.Load(json);
            #endregion
        }

        public static string GetQuery(string name, Dictionary<string, object> queries)
        {
            if (queries == null)
                throw new NotImplementedException("Local queries is not implemented");

            var foo = queries
                .Where(t => t.Key.Equals(name))
                .Select(t => t.Value).FirstOrDefault();

            if (foo == null)
                throw new KeyNotFoundException($"Query {name} not found");
            else
                return foo.ToString();
        }
    }
}
