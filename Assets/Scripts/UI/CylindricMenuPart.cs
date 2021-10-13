using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace UI
    {
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

            public void DecreaseSiblingIndex()
            {
                if (siblingIndex > 0)
                {
                    siblingIndex--;
                    transform.SetSiblingIndex(siblingIndex);
                }
            }

            public void IncreaseSiblingIndex()
            {
                if (siblingIndex < transform.parent.childCount-1)
                {
                    siblingIndex++;
                    transform.SetSiblingIndex(siblingIndex);
                }
            }

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

            public void MoveToMenuBank()
            {
                isPinned = false;
                isOpen = false;
                gameObject.transform.SetParent(refCylindricMenuBank.transform);
                gameObject.SetActive(false);
            }

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

            public void MoveToMenuRoot()
            {
                isPinned = true;
                gameObject.transform.SetParent(refCylindricMenuRoot.transform);
                siblingIndex = transform.GetSiblingIndex();
            }

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

