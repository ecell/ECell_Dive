using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// The logic to handle a part of a cylindric menu.
        /// </summary>
        public class CylindricMenuPart : MonoBehaviour
        {
            [Header("General References")]
            public GameObject refCylindricMenuRoot;
            public GameObject refCylindricMenuBank;
            public GameObject refCylindricMenuExplorer;

            [Header("State Variables")]
            public bool isPinned = false;
            public bool isOpen = false;
            public int menuExplorerSiblingIndex = 0;

            private int siblingIndex = 0;

            private void Start()
            {
                siblingIndex = transform.GetSiblingIndex();
            }

            /// <summary>
            /// Decreases the position of the GameObject in the hierarchy
            /// of its siblings.
            /// </summary>
            /// <remarks>While using the <seealso cref="CylindricLayoutGroup"/>
            /// this will have for effect to automatically move the GameObject to
            /// the left.</remarks>
            public void DecreaseSiblingIndex()
            {
                if (siblingIndex > 0)
                {
                    siblingIndex--;
                    transform.SetSiblingIndex(siblingIndex);
                }
            }

            /// <summary>
            /// Increases the position of the GameObject in the hierarchy
            /// of its siblings.
            /// </summary>
            /// <remarks>While using the <seealso cref="CylindricLayoutGroup"/>
            /// this will have for effect to automatically move the GameObject to
            /// the right.</remarks>
            public void IncreaseSiblingIndex()
            {
                if (siblingIndex < transform.parent.childCount-1)
                {
                    siblingIndex++;
                    transform.SetSiblingIndex(siblingIndex);
                }
            }

            /// <summary>
            /// Switches the position the menu part between outside the menu
            /// explorer and inside the menu explorer.
            /// </summary>
            public void ManagePinning()
            {
                switch (isPinned)
                {
                    case true:
                        isPinned = false;
                        MoveToMenuExplorer();
                        break;

                    case false:
                        isPinned = true;
                        MoveToMenuRoot();
                        break;
                }
            }

            /// <summary>
            /// Hides the menu part in the background.
            /// </summary>
            public void MoveToMenuBank()
            {
                isPinned = false;
                isOpen = false;
                gameObject.transform.SetParent(refCylindricMenuBank.transform);
                gameObject.SetActive(false);
            }

            /// <summary>
            /// Makes the menu part visible in the menu explorer.
            /// </summary>
            public void MoveToMenuExplorer()
            {
                gameObject.SetActive(true);
                isOpen = true;
                gameObject.transform.SetParent(refCylindricMenuExplorer.transform);

                //Necessary to reset rotation and position in the case the menu comes
                //from the the "Pinned" space. 
                gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x,
                                                                 gameObject.transform.localPosition.y,
                                                                 0f);
                gameObject.transform.localRotation = Quaternion.identity;

                siblingIndex = menuExplorerSiblingIndex;
                transform.SetSiblingIndex(siblingIndex);
            }

            /// <summary>
            /// Puts the menu part outside of the menu explorer to keep it always visible.
            /// </summary>
            public void MoveToMenuRoot()
            {
                isPinned = true;
                gameObject.transform.SetParent(refCylindricMenuRoot.transform);
                siblingIndex = transform.GetSiblingIndex();
            }

            /// <summary>
            /// Called back when another UI element was interacted with in order to
            /// show this menu part. The menu part is showed in the menu explorer
            /// by default.
            /// </summary>
            public void OpenMenu()
            {
                if (!isPinned)
                {
                    MoveToMenuExplorer();
                }
            }
        }
    }
}

