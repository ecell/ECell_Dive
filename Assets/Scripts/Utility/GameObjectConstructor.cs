using UnityEngine;
using ECellDive.Utility;

namespace ECellDive
{
    namespace Utility
    {
        /// <summary>
        /// Public interface to add module data on callback in
        /// in the Unity Editor.
        /// </summary>
        public class GameObjectConstructor : MonoBehaviour
        {
            public GameObject refPrefab;

            public void Constructor()
            {
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0f);
                Instantiate(refPrefab, pos, Quaternion.identity);
            }
        }
    }
}

