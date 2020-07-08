using System;
using System.Collections.Generic;
using System.Text;

namespace KSInterfaces
{
    public interface IBaseModel
    {
        void Load(string json);
        string ToJson();
    }
}
