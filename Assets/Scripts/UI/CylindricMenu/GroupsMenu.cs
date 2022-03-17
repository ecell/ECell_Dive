using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellDive.Interfaces;
using ECellDive.SceneManagement;
using TMPro;


namespace ECellDive
{
    namespace UI
    {
        public class GroupsMenu : MonoBehaviour
        {
            public GameObject semanticTermUIPrefab;
            public GameObject groupUIPrefab;
            public GameObject allUIContainer;

            private void Awake()
            {
                GroupsManagement.refGroupsMenu = this;
            }

            private void Start()
            {
                //GameObject semanticTermUI = Instantiate(semanticTermUIPrefab, allUIContainer.transform);
                //semanticTermUI.SetActive(true);
                //semanticTermUI.GetComponent<SemanticGroupManager>().SetData(new SemanticGroupData
                //{
                //    isActive = false,
                //    name = "Custom groups"
                //});
            }

            private void AddGroupUI(GroupData _groupData, IDropDown _parent)
            {
                GameObject groupUI = Instantiate(groupUIPrefab, allUIContainer.transform);
                //groupUI.SetActive(true);
                groupUI.GetComponent<GroupManager>().SetData(_groupData);
                groupUI.GetComponent<GroupManager>().ForceDistributeColor(true);
                _parent.AddItem(groupUI);
            }

            //private void AddGroupUI(GroupData _groupData, int _siblingIndex)
            //{
            //    GameObject groupUI = Instantiate(groupUIPrefab, allUIContainer.transform);
            //    groupUI.transform.SetSiblingIndex(_siblingIndex);
            //    groupUI.SetActive(true);
            //    groupUI.GetComponent<GroupManager>().SetData(_groupData);
            //    groupUI.GetComponent<GroupManager>().ForceDistributeColor(true);
            //}

            public void AddSemanticTermUI(string _semanticTerm, List<GroupData> _groupsData)
            {
                GameObject semanticTermUI = Instantiate(semanticTermUIPrefab, allUIContainer.transform);
                semanticTermUI.SetActive(true);
                semanticTermUI.GetComponentInChildren<TMP_Text>().text = _semanticTerm;

                SemanticGroupManager refSGM = semanticTermUI.GetComponent<SemanticGroupManager>();
                //refSGM.SetData(_semanticGroupData);

                foreach(GroupData _groupData in _groupsData)
                {
                    AddGroupUI(_groupData, refSGM);
                }
            }
        }
    }
}

