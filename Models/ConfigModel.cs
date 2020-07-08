using System;
using System.Collections.Generic;
using System.Text;

namespace KS.Models
{
    internal class ConfigModel :  KSBase.ConfigFile
    {
        public string MasterKey
        {
            get => _Properties.Get(nameof(MasterKey));
            set => _Properties.Set(nameof(MasterKey), value);
        }
    }
}
