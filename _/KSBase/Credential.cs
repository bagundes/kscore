using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace KSBase
{
    public abstract class Credential
    {
        private string masterKey => KSGlobal.App.MasterKey;

        public string Host { get; internal set; }
        public string User { get; internal set; }
        public string EPassword { get; internal set; }
        public DateTime Validate { get; internal set; }

        public Credential(string host, string user, string password)
        {
            this.Host = host;
            this.User = user;
            this.EPassword = KS.Security.Encrypt(password, host + user + masterKey);
        }

        public string Serialize(DateTime validate)
        {
            Validate = validate;
            XmlSerializer xsSubmit = new XmlSerializer(this.GetType());
            var subReq = this;
            
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    var xml = sww.ToString(); // Your XML
                    return KS.Security.Encrypt(xml);
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
