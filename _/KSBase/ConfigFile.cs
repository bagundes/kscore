using KS;
using KS.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KSBase
{
    public class ConfigFile : KSInterfaces.IConfig
    {
        [JsonIgnore]
        protected Bucket _Properties = new Bucket();

        public int LibraryId
        {
            get => _Properties.Get(nameof(LibraryId));
            set => _Properties.Set(nameof(LibraryId), value);
        }

        public string Library
        {
            get => _Properties.Get(nameof(Library));
            set => _Properties.Set(nameof(Library), value);
        }

        public string FullName
        {
            get => _Properties.Get(nameof(FullName));
            set => _Properties.Set(nameof(FullName), value);
        }
        public string Language
        {
            get => _Properties.Get(nameof(Language));
            set => _Properties.Set(nameof(Language), value);
        }

        public ConfigFile() { }
        public ConfigFile(string json)
        {
            Load(json);
        }
        public void Load(string json)
        {
            if (String.IsNullOrEmpty(json))
                return;

            var properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (_Properties is null)
                _Properties = new Bucket(properties);
            else
                _Properties.Set(properties, 0);
        }
        public void Join(string json)
        {
            var properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            _Properties.Set(properties, 0);
        }
        public void Join<T>(T config) where T : KSInterfaces.IConfig
        {
            foreach (var prop in KS.Reflections.GetMembers(config))
                _Properties.Set(prop.Name, Reflections.GetValue(config, prop.Name));
        }
        public Dynamic Get(string property)
        {
            return _Properties.Get(nameof(FullName));
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
