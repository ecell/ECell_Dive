using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace Utility
    {
        public class LivingObject : MonoBehaviour, ILiving
        {
            public virtual GameObject SelfInstance()
            {
                return Instantiate(gameObject);
            }

            public virtual void SelfDestroy()
            {
                Destroy(gameObject);
            }
        }
    }
}

