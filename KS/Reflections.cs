using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace KS
{
    public static class Reflections
    {
        #region Attributes
        public static T GetAttribute<T>(Type t) where T : Attribute, new()
        {
            T MyAttribute = (T)Attribute.GetCustomAttribute(t, typeof(T));

            if (MyAttribute == null)
                return null;
            else
                return MyAttribute;
        }

        public static Dictionary<string, T> GetAttribute<T>(object model) where T : Attribute, new()
        {
            var res = new Dictionary<string, T>();

            foreach (var field in model.GetType().GetFields())
            {
                foreach (object attr in field.GetCustomAttributes(true))
                    if (attr != null && attr is T)
                        res.Add(field.Name, (T)attr);
            }

            return res;
        }
        #endregion

        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {

            var name = assembly.GetName().Name;
            var foo = assembly.GetTypes()
                      .Where(t => t.Namespace.StartsWith(name))
                      .ToArray();


            return foo.Where(t => t.Namespace.Contains($".{nameSpace}")).ToArray();

        }

        public static bool SetValues<T>(T model, Dictionary<string, object> values) where T : KSBase.BaseModel
        {
            foreach (var value in values)
                SetValue(model, value.Key, value.Value);

            return true;
        }

        public static bool SetValue<T>(T model, string name, Object value) where T : KSBase.BaseModel
        {
            try
            {
                // Property
                var name1 = (model.GetType().GetProperties())
                        .Where(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        .Select(t => t.Name).FirstOrDefault();

                var dynValue = new  Dynamic(value);

                if (name1 != null)
                {

                    var proper = model.GetType().GetProperty(name1);
                    if (proper is null || proper.SetMethod == null) return false;

                    if (proper.PropertyType.IsEnum)
                    {
                        var bar = dynValue.ToEnum(proper.PropertyType);
                        proper.SetValue(model, bar);
                    }
                    else
                    {
                        var foo = Type.GetTypeCode(proper.PropertyType);
                        proper.SetValue(model, dynValue.ToType(foo));
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                KSHelpers.DiagnosticHelper.Error(ex);
                return false;
            }

            try
            {
                // Field
                var name1 = (model.GetType().GetFields(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance))
                        .Where(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        .Select(t => t.Name).FirstOrDefault();

                var dynValue = new Dynamic(value);

                if (name1 != null)
                {

                    var field = model.GetType().GetField(name1, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (field is null) return false;

                    if (field.FieldType.IsEnum)
                    {
                        var bar = dynValue.ToEnum(field.FieldType);
                        field.SetValue(model, bar);
                    }
                    else
                    {
                        var foo = Type.GetTypeCode(field.FieldType);
                        field.SetValue(model, dynValue.ToType(foo));
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                KSHelpers.DiagnosticHelper.Error(ex);
                return false;
            }

            return false;
        }

        public static object GetValue<T>(T obj, string name)
        {
            GetValue(obj, name, out object value);
            return value;
        }

        public static bool GetValue<T>(T obj, string name, out object value)
        {
            var p = obj.GetType();
            value = null;

            // Properties
            var key = p.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (key != null)
            {
                value = key.GetValue(obj);
                return true;
            }

            // Fields
            var key1 = p.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (key != null)
            {
                value = key.GetValue(obj);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Verify if the property or field exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns>-1 none, 1 property and 2 field</returns>
        public static int HasPropertyOrField<T>(T obj, string name)
        {
            var p = obj.GetType();

            // Properties
            var key = p.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (key != null)
                return 1;

            // Fields
            var key1 = p.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (key1 != null)
                return 2;
            else
                return -1;
        }

        public static object GetMember<T>(T obj, string name)
        {
            var m = obj.GetType();
            var key = m.GetMember(name);
            var foo = key.GetValue(0);

            return foo;

        }

        public static PropertyInfo[] FilterOnlySetProperties(Object obj)
        {
            var p = obj.GetType();
            var res = new List<PropertyInfo>();

            foreach (var proper in p.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var setter = proper.GetSetMethod();

                if (setter != null)
                    res.Add(proper);
            }

            return res.ToArray();
        }

        #region Fields
        public static FieldInfo[] GetPublicFields(Object obj)
        {
            BindingFlags bindFlags = BindingFlags.Instance
                | BindingFlags.Public
                //| BindingFlags.NonPublic
                | BindingFlags.Static;

            return GetFields(obj, bindFlags);
        }

        public static FieldInfo[] GetPrivateFields(Object obj)
        {
            BindingFlags bindFlags = BindingFlags.Instance
                //| BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Static;

            return GetFields(obj, bindFlags);
        }

        public static FieldInfo[] GetAllFields(Object obj)
        {
            BindingFlags bindFlags = BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Static;

            return GetFields(obj, bindFlags);
        }

        private static FieldInfo[] GetFields(Object obj, BindingFlags bindFlags)
        {
            var p = obj.GetType();
            var res = new List<FieldInfo>();

            foreach (var proper in p.GetFields(bindFlags))
            {

                if (proper != null && proper.Name != "get_LOG")
                    res.Add(proper);
            }

            return res.ToArray();
        }
        #endregion

        public static MemberInfo[] GetMembers(Object obj)
        {
            var p = obj.GetType();
            var res = new List<MemberInfo>();
            var ignore = new string[] { "Equals", "GetHashCode", "GetType", "ToString", ".ctor" };

            foreach (var member in p.GetMembers())
            {
                if (!ignore.Contains(member.Name))
                    res.Add(member);
            }


            return res.ToArray();
        }

        public static PropertyInfo[] GetProperties(Object obj)
        {
            var p = obj.GetType();
            var res = new List<PropertyInfo>();

            foreach (var proper in p.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var getter = proper.GetGetMethod();

                if (getter != null && getter.Name != "get_LOG")
                    res.Add(proper);
            }

            return res.ToArray();
        }

        public static MethodInfo[] GetMethods(Object obj)
        {
            return obj.GetType().GetMethods();
        }

        public static MethodInfo GetPrivateMethod(Object obj, string name)
        {
            return obj.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(t => t.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
        #region StackFrame
        private const int FrameDefault = 3;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static StackFrame GetStackFrame(int frame = FrameDefault)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(frame);

            var foo = sf.GetMethod();
            var bar = foo.Module.Name;

            return sf;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static (string method, string @class) GetClassDetails(int frame = FrameDefault)
        {
            var sf = GetStackFrame(frame + 1);
            var method = sf.GetMethod().Name;
            var @class = sf.GetMethod().DeclaringType.Name;
            var named = (method: method, @class: @class);
            return named;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetClassFullDetails(int frame = FrameDefault)
        {
            var sf = GetStackFrame(frame + 1);
            var method = sf.GetMethod().Name;
            var @class = sf.GetMethod().DeclaringType.FullName;
            var named = $"{@class}.{method}";
            return named;
        }
        #endregion
    }
}
