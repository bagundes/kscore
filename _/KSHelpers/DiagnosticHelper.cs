using System;
using System.Collections.Generic;
using System.Text;

namespace KSHelpers
{
    public static class DiagnosticHelper
    {
        /// <summary>
        /// Register error message in the log
        /// </summary>
        /// <param name="ex">Exception created</param>
        /// <returns>Id message log</returns>
        public static int Error(Exception ex)
        {
            var log = KS.Reflections.GetClassDetails();
            var code = KS.Security.IdNumber(ex.Message);

            return code;
        }
    }
}
