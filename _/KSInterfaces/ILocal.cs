using KSBase;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace KSInterfaces
{
    public interface ILocal<T>
    {
        Assembly Assembly { get; }
        Message GetMessage(Enum code);
        Message GetMessage(int code);
    }
}
