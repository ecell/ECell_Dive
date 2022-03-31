using System.Collections.Generic;
using UnityEngine;
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

            //public List<GameObject> content { get; protected set; }
            private GameObject m_content;
            public GameObject content
            {
                get => m_content;
                set => m_content = value;
            }

            [SerializeField] private GameObject m_scrollListPrefab;
            public GameObject scrollListPrefab
            {
                get => m_scrollListPrefab;
                set => m_scrollListPrefab = value;
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
            public GameObject AddItem(GameObject _item)
            {
                return scrollList.AddItem(_item);
            }

            public void DisplayContent()
            {
                //foreach (RectTransform _item in content.refContent)
                //{
                //    _item.gameObject.SetActive(true);
                //}
                m_content.SetActive(true);
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.8f);
                m_content.transform.position = pos;
                m_content.GetComponent<FaceCamera>().ShowBackToPlayer();
            }

            public void HideContent()
            {
                //foreach (RectTransform _item in m_content.refContent)
                //{
                //    _item.gameObject.SetActive(false);
                //}
                m_content.SetActive(false);
            }


            public void InstantiateContent()
            {
                m_content = Instantiate(m_scrollListPrefab);
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.8f);
                m_content.transform.position = pos;
                m_content.GetComponent<FaceCamera>().ShowBackToPlayer();
                m_scrollList = m_content.GetComponentInChildren<OptimizedVertScrollList>();
            }
            #endregion
        }
    }
}

