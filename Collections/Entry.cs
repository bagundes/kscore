using System;
using System.Collections.Generic;
using System.Text;

namespace KS.Collections
{
    public struct Entry : ICloneable, IEquatable<Entry>
    {
        public int Index;
        public string Name;
        public Dynamic Value;

        public object Clone()
        {
            return new Entry { Index = Index, Name = Name, Value = Value };
        }

        public bool Equals(Entry entry)
        {
            return Name == entry.Name && Value == entry.Value;
        }

        public override string ToString()
        {
            return $"{Index}) {Name}:{Value}";
        }
    }
}
