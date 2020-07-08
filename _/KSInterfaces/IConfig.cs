using KS;
using System;
using System.Collections.Generic;
using System.Text;

namespace KSInterfaces
{
    public interface IConfig: KSInterfaces.IBaseModel
    {
        int LibraryId { get; set; }
        string Library { get; set; }
        string FullName { get; set; }
        string Language { get; set; }
        void Join(string json);
        void Join<T>(T config) where T : KSInterfaces.IConfig;
        KS.Dynamic Get(string property);
    }
}
