using System;
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace KSBase
{
    public sealed class Message
    {
        public int number;
        public string message;
        public Uri link;

        public Message(int number, string message, string link)
        {
            this.number = number;
            this.message = message;
            if(!String.IsNullOrEmpty(link))
                this.link = new Uri(link);
        }
    }
}
