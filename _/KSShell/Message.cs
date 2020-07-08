using System;
using System.Collections.Generic;
using System.Text;

namespace KSShell
{
    public static class Message
    {
        private static int line = 0;

        private static void FirstLine()
        {
            if (line > 0) return;
        }

        private static void Write(KSEnums.TypeOfMessage message, string text, params object[] values)
        {
            FirstLine();

            line++;
            var hours = DateTime.Now.ToString("hh:MM");
            var tm = message.ToString() + new string(' ', 10 - message.ToString().Length);

            System.Console.WriteLine($"{line:00}.{hours} - {tm}: {KS.Dynamic.StringFormat(text, values)}");
        }


        public static void WriteLine(string text, params object[] values)
        {

        }
    }
}
