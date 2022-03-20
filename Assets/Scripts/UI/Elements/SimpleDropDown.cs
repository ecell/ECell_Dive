using System.Collections.Generic;
using UnityEngine;
using ECellDive.Interfaces;


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
            public bool isExpanded { get; protected set; }
            public List<GameObject> content { get; protected set; }
            #endregion

            private void Awake()
            {
                isExpanded = false;
                content = new List<GameObject>();
            }

            /// <summary>
            /// To be called back when the user wants to expand of collapse the
            /// view of the groups stored in the container.
            /// </summary>
            public void ManageExpansion()
            {
                if (isExpanded)
                {
                    refDropDownImageExpanded.SetActive(false);
                    refDropDownImageCollapsed.SetActive(true);
                    HideContent();
                }
                else
                {
                    refDropDownImageCollapsed.SetActive(false);
                    refDropDownImageExpanded.SetActive(true);
                    DisplayContent();
                }
                isExpanded = !isExpanded;
            }

            #region - IDropDown Methods -
            public void AddItem(GameObject _item)
            {
                content.Add(_item);
            }

            public void DisplayContent()
            {
                foreach(GameObject _item in content)
                {
                    _item.SetActive(true);
                }
            }

            public void HideContent()
            {
                foreach (GameObject _item in content)
                {
                    _item.SetActive(false);
                }
            }
            #endregion
        }
    }
}

