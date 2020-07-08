using KSHelpers;
using KSInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KSBase
{
    public abstract class Local<T> where T : KSInterfaces.IConfig, new()
    {
        public T Config;
        private List<KSBase.Message> messages;
        private Dictionary<string, object> queries;
        public  Assembly Assembly { get; internal set; }
        public Local(Assembly assembly)
        {
            Assembly = assembly;
            var config = new T();
            messages = LocalHelper.LoadMessages(Assembly);
            LocalHelper.LoadConfig(Assembly, ref config);
            Config = config;
        }
        public KSBase.Message GetMessage(Enum code)
        {
            return GetMessage(Convert.ToInt32(code));
        }
        public KSBase.Message GetMessage(int code)
        {
            return messages.Where(t => t.number == code).FirstOrDefault();
        }

        
    }

    public abstract class Parameters { }
}
