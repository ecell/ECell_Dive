using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Interfaces;
using ECellDive.Utility;


namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Manages a GUI element behaving like a drop down.
        /// </summary>
        public class SimpleDropDown : MonoBehaviour,
                                             IDropDown
        {
            #region - IDropDown Members -
            [SerializeField]
            private GameObject m_refDropDownImageCollapsed;
            public GameObject refDropDownImageCollapsed
            {
                get => m_refDropDownImageCollapsed;
                set => m_refDropDownImageCollapsed = value;
            }
            [SerializeField]
            private GameObject m_refDropDownImageExpanded;
            public GameObject refDropDownImageExpanded
            {
                get => m_refDropDownImageExpanded;
                set => m_refDropDownImageExpanded = value;
            }

            private bool m_isExpanded = false;
            public bool isExpanded
            {
                get => m_isExpanded;
                protected set => m_isExpanded = value;
            }

            private GameObject m_content;
            public GameObject content
            {
                get => m_content;
                set => m_content = value;
            }

            [SerializeField] private GameObject m_scrollListHolderPrefab;
            public GameObject scrollListHolderPrefab
            {
                get => m_scrollListHolderPrefab;
                set => m_scrollListHolderPrefab = value;
            }

            [SerializeField] private GameObject m_scrollListPrefab;
            public GameObject scrollListPrefab
            {
                get => m_scrollListPrefab;
                set => m_scrollListPrefab = value;
            }

            private GameObject m_scrollListHolder;
            public GameObject scrollListHolder
            {
                get => m_scrollListHolder;
                set => m_scrollListHolder = value;
            }

            private OptimizedVertScrollList m_scrollList;
            public OptimizedVertScrollList scrollList
            {
                get => m_scrollList;
                set => m_scrollList = value;
            }
            #endregion

            /// <summary>
            /// To be called back when the user wants to expand of collapse the
            /// view of the groups stored in the container.
            /// </summary>
            public void ManageExpansion()
            {
                isExpanded = !isExpanded;
                if (isExpanded)
                {
                    refDropDownImageCollapsed.SetActive(false);
                    refDropDownImageExpanded.SetActive(true);
                    DisplayContent();
                }
                else
                {
                    refDropDownImageExpanded.SetActive(false);
                    refDropDownImageCollapsed.SetActive(true);
                    HideContent();
                }
            }

            #region - IDropDown Methods -
            public GameObject AddItem()
            {
                return scrollList.AddItem();
            }

            public void DisplayContent()
            {
                m_content.SetActive(true);

                //Repositioning the scrollListHolder in front of the user.
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.8f);
                m_scrollListHolder.transform.position = pos;
                m_scrollListHolder.GetComponent<FaceCamera>().ShowBackToPlayer();
            }

            public void HideContent()
            {
                m_content.SetActive(false);
            }

            public void InstantiateContent()
            {
                m_scrollListHolder = Instantiate(m_scrollListHolderPrefab);
                m_content = Instantiate(m_scrollListPrefab);
                m_content.transform.parent = m_scrollListHolder.transform;
                m_scrollListHolder.GetComponent<XRGrabInteractable>().colliders.Add(m_content.GetComponentInChildren<BoxCollider>());
                m_scrollListHolder.GetComponent<XRGrabInteractable>().enabled = true;

                //Positioning the scrollListHolder in front of the user.
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.8f);
                m_scrollListHolder.transform.position = pos;
                m_scrollListHolder.GetComponent<FaceCamera>().ShowBackToPlayer();

                m_scrollList = m_content.GetComponentInChildren<OptimizedVertScrollList>();
            }
            #endregion
        }
    }
}

