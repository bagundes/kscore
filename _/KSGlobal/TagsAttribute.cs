using System;

namespace KSGlobal
{
    internal class ValueAttribute : Attribute
    {
        public readonly string value;
        public ValueAttribute(string val)
        {
            value = val;
        }
    }

    internal class PersonalTagAttribute : Attribute
    {
        public readonly string value;
        public PersonalTagAttribute(string val)
        {
            value = val;
        }
    }
}