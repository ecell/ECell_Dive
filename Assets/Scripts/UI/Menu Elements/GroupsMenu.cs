using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.SceneManagement;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Manages the GUI that displays every group either produced by a "GroupByModule"
        /// or made out of selected objects by the user.
        /// </summary>
        public class GroupsMenu : MonoBehaviour
        {
            public GameObject refAllUIContainer;

            public OptimizedVertScrollList semanticGroupsScrollList;
            private List<IDropDown> allDropDowns = new List<IDropDown>();

            public bool hideOnStart = true;

            private void Awake()
            {
                //Making a global reference to this script so that it can be easily
                //accessed from outside without having to resort to FindObjectOfType<>
                //and the like.
                GroupsManagement.refGroupsMenu = this;
            }

            private void Start()
            {
                AddSemanticTermUI("Custom Groups", new List<GroupData>() { });
                if (hideOnStart)
                {
                    gameObject.SetActive(false);
                }
            }

            /// <summary>
            /// Adds a <see cref="groupUIPrefab"/> object at the end of the 
            /// <see cref="allUIContainer"/>.
            /// </summary>
            /// <param name="_groupData">The data needed to correctly initialize
            /// the <see cref="groupUIPrefab"/>.</param>
            /// <param name="_parent">The reference to the drop down the new
            /// <see cref="groupUIPrefab"/> should be part of.</param>
            private void AddGroupUI(GroupData _groupData, IDropDown _parent)
            {
                GameObject groupUI = _parent.AddItem();
                groupUI.GetComponent<GroupUIManager>().SetData(_groupData);
                groupUI.GetComponent<GroupUIManager>().ForceDistributeColor(true);
                groupUI.SetActive(true);
                _parent.scrollList.UpdateScrollList();
            }

            /// <summary>
            /// Adds a <see cref="groupUIPrefab"/> object to the drop down with 
            /// index <paramref name="_parentIndex"/> in the list
            /// <see cref="allDropDowns"/>.
            /// </summary>
            /// <param name="_groupData">The data needed to correctly initialize
            /// the <see cref="groupUIPrefab"/>.</param>
            /// <param name="_parentIndex">The index of the parent drop down in 
            /// which we want to insert the newly created <see cref="groupUIPrefab"/>.</param>
            /// <remarks>This is useful when we want to add a <see cref="groupUIPrefab"/>
            /// to a previously created <see cref="semanticTermUIPrefab"/>.</remarks>
            public void AddGroupUI(GroupData _groupData, int _parentIndex)
            {
                IDropDown parent = allDropDowns[_parentIndex];

                GameObject groupUI = parent.AddItem();
                groupUI.GetComponent<GroupUIManager>().SetData(_groupData);
                groupUI.GetComponent<GroupUIManager>().ForceDistributeColor(true);

                groupUI.SetActive(true);

                parent.scrollList.UpdateScrollList();
            }

            /// <summary>
            /// Adds a GUI element that acts as a container (drop down) of several groups.
            /// </summary>
            /// <param name="_semanticTerm">The name of the container.</param>
            /// <param name="_groupsData">The data of every group that falls in
            /// the container.</param>
            /// <remarks>Typically usefull to manage every group produced by
            /// a "Group By" operation.</remarks>
            public void AddSemanticTermUI(string _semanticTerm, List<GroupData> _groupsData)
            {
                //Creating the drop down button
                GameObject semanticTermUI = semanticGroupsScrollList.AddItem();
                semanticTermUI.SetActive(true);
                semanticTermUI.GetComponentInChildren<TMP_Text>().text = _semanticTerm;
                IDropDown ddComponent = semanticTermUI.GetComponent<IDropDown>();

                //Creating the scroll list menu that will contain the objects
                //controlled by the drop down.
                ddComponent.InstantiateContent();
                ddComponent.content.transform.SetParent(refAllUIContainer.transform, false);
                allDropDowns.Add(ddComponent);

                foreach (GroupData _groupData in _groupsData)
                {
                    AddGroupUI(_groupData, ddComponent);
                }
                semanticGroupsScrollList.UpdateScrollList();
                semanticGroupsScrollList.UpdateAllChildrenVisibility();
            }
        }
    }
}

