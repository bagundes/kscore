using KSBase;
using KSInterfaces;
using KS.Models;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace KS
{

    internal class Local : Local<ConfigModel>, ILocal<ConfigModel>
    {
        #region local

        public readonly LocalParameters Parameters;

        public Local() : base(Assembly.GetCallingAssembly())
        {
            Parameters = new LocalParameters(ref Config);
        }
        #endregion

        #region parameters
        public class LocalParameters : KSBase.Parameters
        {

            private readonly Models.ConfigModel config;

            public LocalParameters(ref Models.ConfigModel configModel)
            {
                config = configModel;
            }
        }
        #endregion
    }
}
