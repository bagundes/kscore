using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KSBase
{
    public abstract class BaseModel : KSInterfaces.IBaseModel
    {
        public virtual void Load(string json)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loading by Bucket
        /// </summary>
        /// <param name="bucket">Bucket</param>
        /// <param name="index">Index to read</param>
        public virtual void Load(KS.Collections.Bucket bucket, int index = 0)
        {
            foreach (var column in bucket[index])
                KS.Reflections.SetValue(this, column.Key, column.Value);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
