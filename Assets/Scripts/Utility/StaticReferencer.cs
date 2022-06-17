using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using HSVPicker;


namespace ECellDive.Utility
{
    public class StaticReferencer : NetworkBehaviour
    {
        public static StaticReferencer Instance;

        [Header("Global UI Elements")]
        /// <summary>
        /// Idx 0 --> Virtual Keyboard;
        /// Idx 1 --> ColorPicker
        /// </summary>
        public GameObject refVirtualKeyboard;
        public ColorPicker refColorPicker;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }
    }
}

