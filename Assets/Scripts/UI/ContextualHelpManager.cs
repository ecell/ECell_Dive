using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Class to control the visibility of all contextual info tags
        /// that could be attached to children gameobjects.
        /// </summary>
        public class ContextualHelpManager : MonoBehaviour
        {
            private InfoTagManager[] infoTagManagers;

            public void ContextualHelpGlobalHide()
            {
                foreach (InfoTagManager _infoTag in infoTagManagers)
                {
                    _infoTag.GlobalHide();
                }
            }

            private void Start()
            {
                infoTagManagers = GetComponentsInChildren<InfoTagManager>();
                ContextualHelpGlobalHide();
            }
        }
    }
}

