using System;
using System.Collections.Generic;
using System.Text;

namespace KS
{
    public class KSException : KSBase.Exception
    {

        internal KSException(System.Exception inner, Enums.MessageCode msg, params object[] values) : base(inner, msg, values)
        {
            MessageTemplate = Init.Local.GetMessage(msg);
            MessageTemplate.message = KS.Dynamic.StringFormat(MessageTemplate.message, values);
        }


        internal KSException(Exception ex) : base(ex, Enums.MessageCode.FatalError_1, ex.Message)
        {
            MessageTemplate = Init.Local.GetMessage(Enums.MessageCode.FatalError_1);
            MessageTemplate.message = KS.Dynamic.StringFormat(MessageTemplate.message, ex.Message);
        }
    }
}
