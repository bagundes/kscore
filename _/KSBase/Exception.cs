using System;
using System.Collections.Generic;
using System.Text;

namespace KSBase
{
    public abstract class Exception : System.Exception
    {
        public readonly int Code;
        public readonly string LogName;
        public readonly string Tag;
        protected object[] Values;
        protected KSBase.Message MessageTemplate;

        public override string Message => KS.Dynamic.StringFormat(MessageTemplate.message, Values);
        public Exception(System.Exception inner, Enum msg, params object[] values) : base ($"Error catched by library", inner)
        {
            LogName = KS.Reflections.GetClassDetails().method;
            Code = Convert.ToInt32(msg);
            Tag = msg.ToString();
            Values = values ?? new object[0];

            var stack = new System.Diagnostics.StackFrame();
        }

    }
}
