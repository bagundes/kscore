using KS;
using System;
using System.Collections.Generic;
using System.Text;

namespace KSInterfaces
{
    public interface IDynamic
    {
        bool Equals(KS.Dynamic other);
        bool IsEmpty();
        bool IsFalse();
        bool IsInteger();
        bool IsNumber();
        bool IsTrue();
        bool ToBool();
        T ToEnum<T>() where T : Enum;
        int ToInt(int def = 0);
        double ToDouble(int? digits = null);
        string ToString();
        string ToNString();
    }
}
