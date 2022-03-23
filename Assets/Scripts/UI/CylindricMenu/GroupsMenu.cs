using System.Collections.Generic;
using UnityEngine;
using ECellDive.Interfaces;
using ECellDive.SceneManagement;
using TMPro;


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
            public GameObject semanticTermUIPrefab;
            public GameObject groupUIPrefab;
            public GameObject allUIContainer;

            private void Awake()
            {
                //Making a global reference to this script so that it can be easily
                //accessed from outside without having to resort to FindObjectOfType<>
                //and the like.
                GroupsManagement.refGroupsMenu = this;
            }

            private void Start()
            {
                GameObject semanticTermUI = Instantiate(semanticTermUIPrefab, allUIContainer.transform);
                semanticTermUI.SetActive(true);
                semanticTermUI.GetComponentInChildren<TMP_Text>().text = "Custom Groups";
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
                GameObject groupUI = Instantiate(groupUIPrefab, allUIContainer.transform);
                groupUI.GetComponent<GroupUIManager>().SetData(_groupData);
                groupUI.GetComponent<GroupUIManager>().ForceDistributeColor(true);
                _parent.AddItem(groupUI);
            }

            /// <summary>
            /// Adds a <see cref="groupUIPrefab"/> object right after the object
            /// with sibling index <paramref name="_parentIndex"/> in the gameobject
            /// <see cref="allUIContainer"/>.
            /// </summary>
            /// <param name="_groupData">The data needed to correctly initialize
            /// the <see cref="groupUIPrefab"/>.</param>
            /// <param name="_parentIndex">The sibling index of the object after
            /// which we want to insert the newly created <see cref="groupUIPrefab"/>.</param>
            /// <remarks>This is useful when we want to add a <see cref="groupUIPrefab"/>
            /// to a previously created <see cref="semanticTermUIPrefab"/>.</remarks>
            public void AddGroupUI(GroupData _groupData, int _parentIndex)
            {
                GameObject groupUI = Instantiate(groupUIPrefab, allUIContainer.transform);
                groupUI.transform.SetSiblingIndex(_parentIndex+1);
                groupUI.GetComponent<GroupUIManager>().SetData(_groupData);
                groupUI.GetComponent<GroupUIManager>().ForceDistributeColor(true);

                IDropDown parent = allUIContainer.transform.GetChild(_parentIndex).gameObject.GetComponent<IDropDown>();
                parent.AddItem(groupUI);

                groupUI.SetActive(parent.isExpanded);
            }

            /// <summary>
            /// Adds a GUI element that acts as a container (drop down) of several groups.
            /// Typically usefull to manage every group produced by a "Group By" operation.
            /// </summary>
            /// <param name="_semanticTerm">The name of the container.</param>
            /// <param name="_groupsData">The data of every group that falls in
            /// the container.</param>
            public void AddSemanticTermUI(string _semanticTerm, List<GroupData> _groupsData)
            {
                GameObject semanticTermUI = Instantiate(semanticTermUIPrefab, allUIContainer.transform);
                semanticTermUI.SetActive(true);
                semanticTermUI.GetComponentInChildren<TMP_Text>().text = _semanticTerm;

                SemanticGroupUIManager refSGM = semanticTermUI.GetComponent<SemanticGroupUIManager>();

                foreach(GroupData _groupData in _groupsData)
                {
                    AddGroupUI(_groupData, refSGM);
                }
            }
        }
    }
}

