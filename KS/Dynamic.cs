using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace KS
{
    public partial class Dynamic : IEquatable<Dynamic>, KSInterfaces.IDynamic // Construct
    {
        public readonly object Value;
        public readonly TypeCode TCode;

        public Dynamic()
        {
            Value = null;
        }
        public Dynamic(object value)
        {
            while (value is Dynamic)
                value = ((Dynamic)value).Value;

            Value = value;

            if (value is null)
                TCode = TypeCode.Empty;
            else
                TCode = Type.GetTypeCode(value.GetType());
        }

        #region Implicit conversion
        public static implicit operator Dynamic(string v)
        {
            return new Dynamic(v);
        }

        public static implicit operator string(Dynamic v)
        {
            return v is null ? String.Empty : v.ToString();
        }

        public static implicit operator Dynamic(int v)
        {
            return new Dynamic(v);
        }

        public static explicit operator Dynamic(Int64 v)
        {
            return new Dynamic(v);
        }

        public static implicit operator int(Dynamic v)
        {
            return v.ToInt(0);
        }

        public static implicit operator Dynamic(DateTime v)
        {
            return new Dynamic(v);
        }

        public static implicit operator DateTime(Dynamic v)
        {
            return v.ToDateTime();
        }

        public static implicit operator Dynamic(bool v)
        {
            return new Dynamic(v);
        }

        public static implicit operator Dynamic(double v)
        {
            return new Dynamic(v);
        }


        #endregion

    }
    // Check
    public partial class Dynamic : IEquatable<Dynamic>, KSInterfaces.IDynamic // Check
    {
        #region Number
        public bool IsNumber()
        {
            if (IsInteger()
                || Regex.IsMatch(ToString(), @"\d+"))
                return true;
            else
                return false;
        }
        public bool IsInteger()
        {
            switch (TCode)
            {
                case System.TypeCode.Int16:
                case System.TypeCode.Int32:
                case System.TypeCode.Int64:
                case System.TypeCode.UInt16:
                case System.TypeCode.UInt32:
                case System.TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region String
        public bool IsChar()
        {
            return TCode == TypeCode.Char;
        }
        public bool IsEmpty()
        {
            return Value == null || String.IsNullOrEmpty(Value.ToString());
        }
        #endregion

        #region Boolean
        public bool IsFalse()
        {
            return !ToBool();
        }
        public bool IsTrue()
        {
            return ToBool();
        }
        #endregion

        #region Others
        public string GetOnlyNumbers(int def = 0)
        {
            var ret = String.Empty;
            var numbers = "1234567890";

            foreach (var c in this.ToString())
            {
                if (numbers.Contains(c.ToString()))
                    ret += c.ToString();
            }

            return ret;
        }
        public dynamic ToType(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case System.TypeCode.Boolean:
                    return ToBool();
                case System.TypeCode.Int16:
                case System.TypeCode.Int32:
                case System.TypeCode.Int64:
                case System.TypeCode.UInt16:
                case System.TypeCode.UInt32:
                case System.TypeCode.UInt64:
                    return ToInt();
                case TypeCode.String:
                    return ToString();

                default:
                    return Value;
            }
        }
        public int ToEnum(Type type)
        {
            if (IsInteger())
                return (int)Enum.ToObject(type, ToInt());
            else
            {
                var list = Enum.GetNames(type);
                var value = list.Where(t => t.Equals(ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                return (int)Enum.Parse(type, value);
            }
        }
        #endregion

        #region SAP
        public TimeSpan Sap_ToTime()
        {
            int hours, minutes, seconds;
            var time = ToInt();
            var size = time.ToString().Length;

            if(size > 4)
            {
                hours = ((int)time / 10000);
                time -= (hours * 10000);
                minutes = ((int)time / 100);
                seconds = ((int)time % 100);
            } else
            {
                var _ = 100;
                hours = ((int)time / _);
                minutes = ((int)time % _);
                seconds = 0;
            }

            return new TimeSpan(hours, minutes, seconds);
        }

        /// <summary>
        /// Format sap time to hh:mm
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public string Sap_ToTime_Formated(bool seconds = false)
        {
            var ts = Sap_ToTime();
            if (seconds)
                return $"{(int)ts.TotalHours}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
            else
                return $"{(int)ts.TotalHours}:{ts.Minutes.ToString("00")}";
        }
        #endregion
    }
    //Convert
    public partial class Dynamic : IEquatable<Dynamic>, KSInterfaces.IDynamic // Convert
    {
        public char ToChar()
        {
            if (IsChar())
                return (char)Value;
            if (IsInteger())
                return (char)ToInt();
            else
                throw new Exception($"It not possible convert {Value} to char");
        }
        public T ToEnum<T>() where T : Enum
        {
            var type = typeof(T);

            if (IsInteger())
                return (T)Enum.ToObject(type, ToInt());
            if (IsChar())
                return (T)Enum.ToObject(type, (int)Char.GetNumericValue(ToChar())); 
            else
            {
                var list = Enum.GetNames(type);
                var value = list.Where(t => t.Equals(ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                return (T)Enum.Parse(type, value);
            }
        }
        public string ToNString()
        {
            return Value == null ? null : Value.ToString().Trim();
        }
        public override string ToString()
        {
            return Value == null ? String.Empty : Value.ToString().Trim();
        }
        public int ToInt(int def = 0) => int.TryParse(this.ToString(), out int val) ? val : def;
        public double ToDouble(int? digits = null)
        {
            if(double.TryParse(this.ToString(), out double res))
            {
                if (digits.HasValue)
                    res = Math.Round(res, digits.Value);

                return res;
            } else
            {
                return 0;
            }
        }
        public bool ToBool()
        {
            if (TCode == TypeCode.Empty) return false;
            if (ToInt(-1) > 0) return true;

            switch (Value.ToString().ToUpper())
            {
                case "T":
                case "TRUE":
                case "Y":
                case "YES":
                case "S":
                case "SI":
                case "SIM":
                case "1":
                    return true;
                default:
                    return false;
            }
        }
        public DateTime ToDateTime(string format = "yyyy-MM-dd HH:mm:ss")
        {
            if (Value is DateTime)
                return (DateTime)Value;
            else
            {
                if (!String.IsNullOrEmpty(Value.ToString()))
                    return DateTime.ParseExact(Value.ToString(), format,
                                       System.Globalization.CultureInfo.InvariantCulture);
                else
                    return new DateTime();
            }

        }

        /// <summary>
        /// Convert number type double to time. Example: 3.5 = 3h30m.
        /// </summary>
        /// <param name="show_seconds">Format wit seconds</param>
        /// <returns>Example: 3.5 = 330 </returns>
        public TimeSpan DoubleToTime()
        {
            var _ = this.ToDouble();

            var hours = (int)_;
            var minutes = 60 * (_ % (hours == 0 ? 1 : hours));
            var seconds = 60 * (minutes % (int)(minutes == 0 ? 1 : minutes));

            return  new TimeSpan(hours, (int)minutes, (int)seconds);

        }

        public DirectoryInfo ToDirectory()
        {
            var path = ToString();
            if (Directory.Exists(path))
                return new DirectoryInfo(path);
            else
                throw new KSException(null, Enums.MessageCode.DirectoryNotExists_1, path);
        }
        public FileInfo ToFile()
        {
            var path = ToString();
            if (File.Exists(path))
                return new FileInfo(path);
            else
                throw new KSException(null, Enums.MessageCode.FileNotExists_1, path);
        }

        public string FromBase64ToString()
        {
            var base64EncodedBytes = System.Convert.FromBase64String(ToString());
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public string ToBase64String()
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(ToString());
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
    public partial class Dynamic : IEquatable<Dynamic>, KSInterfaces.IDynamic // Construct
    {
        public bool Equals(Dynamic other)
        {
            return ToString().Equals(other.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
    public partial class Dynamic : IEquatable<Dynamic>, KSInterfaces.IDynamic // Static
    {
        public static Dynamic Empty = new Dynamic(null);
        public static Dynamic From(object value) => new Dynamic(value);
        public static string RemoveDuplicateChars(string val)
        {
            var list = new List<char>();

            foreach (var c in val)
            {
                if (!list.Contains(c))
                    list.Add(c);
            }

            return String.Join("", list.ToArray());

        }
        public static string StringFormat(string format, params object[] values)
        {
            var val1 = format.Clone().ToString();

            values = values ?? new object[0];
            int sf = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (format.Contains($"{{{i}}}"))
                {
                    sf++;
                    format = format.Replace($"{{{i}}}", String.Format("{0}", values[i]));
                }
            }

            if (sf != values.Length)
            {
                var list = new List<object>
                {
                    $"Value : {val1}",
                    $"Format: {format}"
                };

                for (int i = 0; i < values.Length; i++)
                    list.Add($"{String.Format("{0, 6}", i)}: {values[i]}");

                //var track = Helpers.DiagnosticHelper.TrackMessages(list.ToArray());
                //Helpers.DiagnosticHelper.Warning("StringFormat", track, "The string is not contains all values of parameters");

                return $"!{format}";
            }

            Plural(format, out string formated);
            format = formated;

            return format;
        }

        /// <summary>
        /// String format with plural option.
        /// Example: "Total {0} #[{0}|euro|euros]". 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="formated"></param>
        /// <returns>Attributes or null</returns>
        private static bool Plural(string text, out string formated)
        {
            try
            {
                var tagStart = "#[";
                var middle = '|';
                var tagEnd = "]#";
                var ret = false;


                if (!text.Contains(tagStart))
                {
                    formated = text;
                    return false;
                }

                var start = text.IndexOf(tagStart);
                var end = text.IndexOf(tagEnd, start);

                if (start >= 0 && end > start)
                {
                    end += tagEnd.Length;
                    var val = text.Substring(start, end - start);

                    var bar = val.Replace(tagStart, "").Replace(tagEnd, "");
                    var foo = bar.Split(middle);

                    if (foo.Length != 3) // Format invalid
                    {
                        formated = text;
                        ret = false;
                    }
                    else
                    {

                        var plural = KS.Dynamic.From(foo[0]).ToBool() ? 2 : 1;

                        formated = text.Replace(val, foo[plural]);

                        ret = true;
                    }
                }
                else
                {
                    formated = text;
                }

                while (ret)
                {
                    ret = Plural(formated, out string foo);
                    formated = foo;
                }



                return ret;
            }
            catch (Exception ex)
            {
                formated = text;
                KSHelpers.DiagnosticHelper.Error(ex);
                return false;
            }
        }
        public static T[] GetEnumAttribute<T>(Enum value) where T : Attribute, new()
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            T[] attributes = fi.GetCustomAttributes(typeof(T), false) as T[];

            return attributes;
        }

        /// <summary>
        /// Convert all values to string
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ConvertToString(params object[] values)
        {

            var input = String.Empty;
            foreach (var val in values)
            {
                if (val.GetType().IsArray)
                {
                    var _ = ((IEnumerable)val).Cast<object>()
                                    .Select(x => x == null ? x : x.ToString())
                                    .ToArray();
                    foreach (var v1 in _)
                        input += v1.ToString();
                }
                else
                    input += val.ToString();
            }

            return input;
        }




    }
}
