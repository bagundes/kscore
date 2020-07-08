using KSInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KS.Collections
{
    public class Bucket : IEnumerable<Dictionary<string, Dynamic>>, IBucket
    {
        private List<Entry> _Entries;

        public Dynamic this[int index, string column]
        {
            get => _Entries.Where(t => t.Name.Equals(column, StringComparison.InvariantCultureIgnoreCase) && t.Index == index)
                            .Select(t => t.Value)
                            .FirstOrDefault();

            set => Set(column, value, index);
        }

        public Dynamic this[string column]
        {
            get => _Entries.Where(t => t.Name.Equals(column, StringComparison.InvariantCultureIgnoreCase))
                            .Select(t => t.Value)
                            .FirstOrDefault();

            set => Set(column, value);
        }

        /// <summary>
        /// Get Entry by index entry
        /// </summary>
        /// <param name="index">Index entry</param>
        /// <returns></returns>
        public Dictionary<string, Dynamic> this[int index] => _Entries.Where(t => t.Index == index).ToDictionary(t => t.Name, t => t.Value);

        public int RowsCount => _Entries is null ? 0 : _Entries.GroupBy(t => t.Index).Count();

        public bool IsEmpty => _Entries is null;

        #region Constructor
        public Bucket()
        {
            _Entries = new List<Entry>();
        }

        public Bucket(Dictionary<string, dynamic> index)
        {
            _Entries = new List<Entry>();

            foreach (var column in index)
                Set(column.Key, 0, column.Value);
        }

        private Bucket(IEnumerable<Entry> entries)
        {
            _Entries = entries.ToList();
        }
        #endregion

        #region Setters
        public void Set(string column, dynamic value)
        {
            Set(column, 0, value);
        }
        public void Set(string column, int row, dynamic value)
        {
            if (Contains(row, column))
            {
                var i = GetRealIndex(row, column);
                var entry = _Entries[i];
                entry.Value = new Dynamic(value);
                _Entries[i] = entry;
            }
            else
            {
                _Entries.Add(new Entry { Index = row, Name = column, Value = new Dynamic(value) });
            }
        }
        public void Set(Dictionary<string, object> data, int row)
        {
            foreach (var val in data)
               Set(val.Key, 0, Dynamic.From(val.Value));
        }
        #endregion

        #region Getters
        public Dynamic Get(string column)
        {
            return Get(column, 0);
        }
        public Dynamic Get(string column, int index)
        {
            if (Exists(column, index))
                return this[index, column];
            else
                return Dynamic.Empty;
        }
        public bool Get(string column, int index, out Dynamic value)
        {
            if (Exists(column, index))
            {
                value = this[index, column];
                return true;
            }
            else
            {
                value = Dynamic.Empty;
                return false;
            }
        }

        /// <summary>
        /// Get line. 
        /// </summary>
        /// <param name="column">Column to find</param>
        /// <param name="value">Value to find</param>
        /// <returns>Entry line</returns>
        public Dictionary<string, Dynamic> GetLine(string column, object value)
        {
            var index = _Entries.Where(t => t.Name.Equals(column, StringComparison.InvariantCultureIgnoreCase) 
                                        && t.Value.ToString().Equals(value))
                .Select(t => t.Index).FirstOrDefault();

            return _Entries.Where(t => t.Index == index).ToDictionary(t => t.Name, t => t.Value);
        }
        public Dynamic[] GetColumnValues(string column)
        {
            return _Entries.Where(t => t.Name.Equals(column, StringComparison.InvariantCultureIgnoreCase))
                .Select(t => t.Value).ToArray();
        }
        #endregion

        private bool Exists(string column, int index)
        {
            return _Entries.Where(t => t.Name.Equals(column, StringComparison.OrdinalIgnoreCase) && t.Index == index).Any();
        }

        /// <summary>
        /// Get real index in Entries list
        /// </summary>
        /// <param name="index">Index entry</param>
        /// <param name="column">Column name</param>
        /// <returns></returns>
        private int GetRealIndex(int index, string column)
        {
            return _Entries.FindIndex(t => t.Name.Equals(column, StringComparison.InvariantCultureIgnoreCase) && t.Index == index);
        }


        #region Validate
        public bool Contains(int index, string column)
        {
            return _Entries != null
                && _Entries.Where(t => t.Index == index && t.Name.Equals(column, StringComparison.InvariantCultureIgnoreCase)).Any();
        }
        #endregion

        #region Enumerator
        private IEnumerator<Dictionary<string, Dynamic>> Entries()
        {
            var indexes = _Entries.Select(t => t.Index).Distinct().OrderBy(t => t).ToArray();
            foreach(var index in indexes)
                yield return this[index];
        }

        public IEnumerator<Dictionary<string, Dynamic>> GetEnumerator() => Entries();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Entry[] ToArray()
        {
            return _Entries.ToArray();
        }

        #endregion
    }
}
