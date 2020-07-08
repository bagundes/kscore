using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KSGlobal
{
    public static class Tags
    {
        public static int a = 1;

        public enum Tag
        {
            [Value("!!")]
            [PersonalTag("--TAGNAMESPACE")]
            NameSpace,
        }


        //public static bool string GetTag(string val, Tag tag)
        //{

        //}

        //private static string GetPersonalTag(string val, string personalTag)
        //{
        //    if(val.Contains(personalTag))
        //    {

        //    }
        //    else
        //}

    }
}
