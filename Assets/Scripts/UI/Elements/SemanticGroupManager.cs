using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ECellDive.Interfaces;


namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Manages the GUI container (drop down) storing every GUI element representing
        /// groups.
        /// </summary>
        public class SemanticGroupManager : MonoBehaviour, IDropDown
        {
            public Toggle refToggle;
            public TMP_Text refNameText;

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
            private List<GameObject> m_content = new List<GameObject> ();
            public List<GameObject> content
            {
                get => m_content;
                protected set => m_content = value;
            }
            #endregion

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

            /// <summary>
            /// To be called back when the user wants to forcefully activate or deactivate
            /// every group stored in the container.
            /// </summary>
            public void OnToggleValueChange()
            {
                foreach(GameObject _child in content)
                {
                    GroupManager refGM = _child.gameObject.GetComponent<GroupManager>();
                    refGM.ForceDistributeColor(refToggle.isOn);
                    refGM.refToggle.interactable = refToggle.isOn;
                }
            }

            #region - IDropDown Methods -
            public void AddItem(GameObject _item)
            {
                content.Add(_item);
            }

            public void DisplayContent()
            {
                foreach (GameObject _item in content)
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
