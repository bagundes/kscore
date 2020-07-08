using System;
using System.Collections.Generic;
using System.Text;

namespace KSInterfaces
{
    public interface IInit
    {
        void Init10_Dependencies();
        void Init20_Config();
        void Init50_Threads();
        void Init60_End();
    }
}
