using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Interfaces;
using ECellDive.UI;
using ECellDive.SceneManagement;

namespace ECellDive
{
    namespace UserActions
    {
        [System.Serializable]
        public struct GroupMakingActionData
        {
            public InputActionReference selection;
        }

        [System.Serializable]
        public struct GroupMakingToolsData
        {
            public GameObject discreteSelector;
            public GameObject volumetricSelector;
            public bool isVolumetric;
            public int targetLayer;
            public float maxDistance;
        }

        public class GroupsMakingManager : MonoBehaviour
        {
            public GameObject refGroupValidationUI;

            public GroupMakingActionData leftGrpMkgActionData;
            public GroupMakingActionData rightGrpMkgActionData;

            public GroupMakingToolsData leftGrpMkgToolsData;
            public GroupMakingToolsData rightGrpMkgToolsData;

            List<GameObject> groupMembers = new List<GameObject>();

            private void Awake()
            {
                leftGrpMkgActionData.selection.action.performed += AddWithLeftController;
            }

            private void OnDestroy()
            {
                leftGrpMkgActionData.selection.action.performed -= AddWithLeftController;
            }

            private void AddMemberToGroup(GameObject _go, IGroupable _goGroupInfo)
            {
                _goGroupInfo.grpMemberIndex = groupMembers.Count;
                groupMembers.Add(_go);
            }

            private void AddWithLeftController(InputAction.CallbackContext _ctx)
            {
                if (leftGrpMkgToolsData.isVolumetric)
                {

                }

                else
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(leftGrpMkgToolsData.discreteSelector.transform.position,
                                        leftGrpMkgToolsData.discreteSelector.transform.forward,
                                        out hitInfo, leftGrpMkgToolsData.maxDistance))
                    {
                        XRGrabInteractable interactable = FindComponent<XRGrabInteractable>(hitInfo.collider.gameObject);
                        if (interactable != null)
                        {
                            int targetLayerMask = 1 << leftGrpMkgToolsData.targetLayer;
                            if((interactable.interactionLayerMask & targetLayerMask) > 0)
                            {
                                ManageGroupMembers(hitInfo.collider.gameObject);
                            }
                        }
                    }
                }
            }

            public void CancelGroup()
            {
                //Reset objects Highlight
                for (int i = 0; i < groupMembers.Count; i++)
                {
                    IHighlightable highlitable = FindComponent<IHighlightable>(groupMembers[i]);
                    highlitable.forceHighlight = false;
                }

                //Reset group members list
                ResetGroupMemberList();

                //Hide the UI dialogue
                refGroupValidationUI.SetActive(false);
            }

            private T FindComponent<T>(GameObject _go)
            {
                T component = _go.GetComponentInChildren<T>();
                if (component == null)
                {
                    component = _go.transform.parent.gameObject.GetComponent<T>();
                }
                return component;
            }

            private void ManageGroupMembers(GameObject _go)
            {
                IGroupable groupable = FindComponent<IGroupable>(_go);
                if (groupable != null)
                {
                    IHighlightable highlightable = FindComponent<IHighlightable>(_go);
                    if (groupable.grpMemberIndex == -1)
                    {
                        AddMemberToGroup(_go, groupable);
                        highlightable.forceHighlight = true;
                    }
                    else
                    {
                        RemoveMemberFromGroup(_go, groupable);
                        highlightable.forceHighlight = false;
                    }

                    ManageUIConfirmationCanvas();
                }
            }

            private void ManageUIConfirmationCanvas()
            {
                if (groupMembers.Count == 0)
                {
                    refGroupValidationUI.SetActive(false);
                }
                else
                {
                    refGroupValidationUI.SetActive(true);
                }
            }

            private void RemoveMemberFromGroup(GameObject _go, IGroupable _goGroupInfo)
            {
                groupMembers.RemoveAt(_goGroupInfo.grpMemberIndex);
                _goGroupInfo.grpMemberIndex = -1;
            }

            private void ResetGroupMemberList()
            {
                foreach(GameObject _go in groupMembers)
                {
                    IGroupable groupable = FindComponent<IGroupable>(_go);
                    groupable.grpMemberIndex = -1;
                }

                groupMembers.Clear();
            }

            public void ValidateGroup()
            {
                //Get Highlightables and reset force highlight
                IHighlightable[] highlitables = new IHighlightable[groupMembers.Count];
                for(int i = 0; i < groupMembers.Count; i++)
                {
                    highlitables[i] = FindComponent<IHighlightable>(groupMembers[i]);
                    highlitables[i].forceHighlight = false;
                }

                //Create a groupUI component
                GroupsManagement.refGroupsMenu.AddGroupUI(new GroupData
                    {
                        value = "New group",
                        color = Random.ColorHSV(),
                        members = highlitables
                    },
                    0);

                //Reset group members list
                ResetGroupMemberList();

                //Hide the UI dialogue
                refGroupValidationUI.SetActive(false);
            }
        }
    }
}

