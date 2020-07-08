using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSGlobal
{
    public static class InitStart
    {
        public static List<string> Registed { get; internal set; } = new List<string>();

        public static void Register(KSInterfaces.IInit init)
        {
            var name = init.GetType().FullName;
            if (Registed.Where(t => t == name).Any()) return;

            try
            {
                init.Init10_Dependencies();
                init.Init20_Config();
                init.Init50_Threads();
                Registed.Add(name);
            }
            catch(Exception ex)
            {
                KSHelpers.DiagnosticHelper.Error(ex);
                throw ex;
            }
        }
    }
}
