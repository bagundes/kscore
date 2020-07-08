using KS;
using KS.Collections;
using System.Collections.Generic;

namespace KSInterfaces
{
    public interface IBucket
    {
        Dictionary<string, KS.Dynamic> this[int index] { get; }
        KS.Dynamic this[string column] { get; set; }
        KS.Dynamic this[int index, string column] { get; set; }

        bool IsEmpty { get; }
        int RowsCount { get; }

        bool Contains(int index, string column);
        IEnumerator<Dictionary<string, KS.Dynamic>> GetEnumerator();
        void Set(string column, dynamic value);
        void Set(string column, int index, dynamic value);
        void Set(Dictionary<string, dynamic> data, int index);

        KS.Dynamic Get(string column);
        KS.Dynamic Get(string column, int index);
        bool Get(string column, int index, out KS.Dynamic value);

        Entry[] ToArray();
    }
}