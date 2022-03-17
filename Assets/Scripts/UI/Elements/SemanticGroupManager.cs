using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ECellDive.Interfaces;


namespace ECellDive
{
    namespace UI
    {
        //public struct SemanticGroupData
        //{
        //    public string name;
        //    public int nbGroups;
        //}
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

            //private SemanticGroupData semanticGroupData;

            //private void Awake()
            //{
            //    isExpanded = false;
            //    content = new List<GameObject>();
            //    Debug.Log($"SGM Awake, content value:{content}");
            //}
            //private void OnEnable()
            //{
            //    isExpanded = false;
            //    content = new List<GameObject>();
            //    Debug.Log($"SGM OnEnable, content value:{content}");
            //}

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

            public void OnToggleValueChange()
            {
                //int siblingIndex = transform.GetSiblingIndex();
                //int child_counter = 0;
                //foreach (Transform _child in transform.parent)
                //{
                //    if (child_counter > siblingIndex && child_counter <= siblingIndex + semanticGroupData.nbGroups)
                //    {
                //        GroupManager refGM = _child.gameObject.GetComponent<GroupManager>();
                //        refGM.ForceDistributeColor(refToggle.isOn);
                //        refGM.refToggle.interactable = refToggle.isOn;
                //    }
                //    child_counter++;
                //}
                foreach(GameObject _child in content)
                {
                    GroupManager refGM = _child.gameObject.GetComponent<GroupManager>();
                    refGM.ForceDistributeColor(refToggle.isOn);
                    refGM.refToggle.interactable = refToggle.isOn;
                }
            }

            //public void SetData(SemanticGroupData _data)
            //{
            //    semanticGroupData = _data;
            //    refNameText.text = semanticGroupData.name;
            //    isExpanded = false;
            //    content = new List<GameObject>();
            //}

            #region - IDropDown Methods -
            public void AddItem(GameObject _item)
            {
                content.Add(_item);
            }

            public void DrawContent()
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
