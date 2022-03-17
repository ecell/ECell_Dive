using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ECellDive.Interfaces;


namespace ECellDive
{
    namespace UI
    {
        public class DropDownSwitchManager : MonoBehaviour,
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
                Debug.Log($"DDSM Awake, content value:{content}");
            }

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
                    DrawContent();
                }
                isExpanded = !isExpanded;
            }

            #region - IDropDown Methods -
            public void AddItem(GameObject _item)
            {
                content.Add(_item);
            }

            public void DrawContent()
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

