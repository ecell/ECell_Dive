using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace Utility
    {
        public interface ILiving
        {
            GameObject SelfInstance();
            void SelfDestroy();
        }
    }
}
