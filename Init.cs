using KSInterfaces;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace KS
{
    public class Init : KSInterfaces.IInit
    {
        internal static Local Local => new Local();

        public void Init10_Dependencies()
        {
            //throw new NotImplementedException();
        }

        public void Init20_Config()
        {
            KSGlobal.App.AddConfig(Local.Config);
        }

        public void Init50_Threads()
        {
            //throw new NotImplementedException();
        }

        public void Init60_End()
        {
            //throw new NotImplementedException();
        }
    }
}
