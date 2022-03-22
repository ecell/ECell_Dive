using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
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
            public InputActionReference switchSelectionMode;
        }

        [System.Serializable]
        public struct GroupMakingToolsData
        {
            public GameObject discreteSelector;
            public VolumetricSelectorManager volumetricSelector;
            public bool isVolumetric;
            public int targetLayer;
            public float maxDistance;
        }

        public class GroupsMakingManager : MonoBehaviour
        {
            public GameObject refUICanvas;
            public TMP_InputField refGroupNameInputField;

            public GroupMakingActionData leftGrpMkgActionData;
            public GroupMakingActionData rightGrpMkgActionData;

            public GroupMakingToolsData leftGrpMkgToolsData;
            public GroupMakingToolsData rightGrpMkgToolsData;

            List<GameObject> groupMembers = new List<GameObject>();

            private void Awake()
            {
                leftGrpMkgActionData.selection.action.started += e => TryAddMemberStart(leftGrpMkgToolsData);
                leftGrpMkgActionData.selection.action.canceled += e => TryAddMemberEnd(leftGrpMkgToolsData);
                leftGrpMkgActionData.switchSelectionMode.action.performed += SwitchLeftSelectionMode;
                
                rightGrpMkgActionData.selection.action.started += e => TryAddMemberStart(rightGrpMkgToolsData);
                rightGrpMkgActionData.selection.action.canceled += e => TryAddMemberEnd(rightGrpMkgToolsData);
                rightGrpMkgActionData.switchSelectionMode.action.performed += SwitchRightSelectionMode;
            }

            private void OnDestroy()
            {
                leftGrpMkgActionData.selection.action.started -= e => TryAddMemberStart(leftGrpMkgToolsData);
                leftGrpMkgActionData.selection.action.canceled -= e => TryAddMemberEnd(leftGrpMkgToolsData);
                leftGrpMkgActionData.switchSelectionMode.action.performed -= SwitchLeftSelectionMode;
                
                rightGrpMkgActionData.selection.action.started -= e => TryAddMemberStart(rightGrpMkgToolsData);
                rightGrpMkgActionData.selection.action.canceled -= e => TryAddMemberEnd(rightGrpMkgToolsData);
                rightGrpMkgActionData.switchSelectionMode.action.performed -= SwitchRightSelectionMode;
            }

            private void AddMemberToGroup(GameObject _go, IGroupable _goGroupInfo)
            {
                _goGroupInfo.grpMemberIndex = groupMembers.Count;
                groupMembers.Add(_go);
            }

            public void CancelGroup()
            {
                //Reset objects Highlight
                for (int i = 0; i < groupMembers.Count; i++)
                {
                    IHighlightable highlitable = FindComponent<IHighlightable>(groupMembers[i]);
                    highlitable.forceHighlight = false;
                    highlitable.UnsetHighlight();
                }

                //Reset group members list
                ResetGroupMemberList();

                //Hide the UI dialogue
                refUICanvas.SetActive(false);
            }

            public void CheckCollision(GameObject _go)
            {
                if (IsObjectInTargetLayer(_go))
                {
                    ManageGroupMembers(_go);
                }
            }

            private T FindComponent<T>(GameObject _go)
            {
                T component = _go.GetComponentInChildren<T>();
                if (component == null)
                {
                    if (_go.transform.parent != null)
                    {
                        component = _go.transform.parent.gameObject.GetComponent<T>();
                    }
                }
                return component;
            }

            private bool IsObjectInTargetLayer(GameObject _go)
            {
                XRGrabInteractable interactable = FindComponent<XRGrabInteractable>(_go);
                Debug.Log(interactable);
                if (interactable != null)
                {
                    int targetLayerMask = 1 << leftGrpMkgToolsData.targetLayer;
                    return (interactable.interactionLayerMask & targetLayerMask) > 0;
                }

                return false;
            }

            private void ManageGroupMembers(GameObject _go)
            {
                IGroupable groupable = FindComponent<IGroupable>(_go);
                Debug.Log(groupable);
                if (groupable != null)
                {
                    IHighlightable highlightable = FindComponent<IHighlightable>(_go);
                    if (groupable.grpMemberIndex == -1)
                    {
                        AddMemberToGroup(_go, groupable);
                        highlightable.forceHighlight = true;
                        highlightable.SetHighlight();
                    }
                    else
                    {
                        RemoveMemberFromGroup(_go, groupable);
                        highlightable.forceHighlight = false;
                        highlightable.UnsetHighlight();
                    }

                    ManageUIConfirmationCanvas();
                }
            }

            private void ManageUIConfirmationCanvas()
            {
                if (groupMembers.Count == 0)
                {
                    refUICanvas.SetActive(false);
                }
                else
                {
                    refUICanvas.SetActive(true);
                }
            }

            private void RemoveMemberFromGroup(GameObject _go, IGroupable _goGroupInfo)
            {
                Debug.Log($"Trying to remove {_go.name} from group with index {_goGroupInfo.grpMemberIndex}");
                groupMembers.RemoveAt(_goGroupInfo.grpMemberIndex);
                for (int i = _goGroupInfo.grpMemberIndex; i < groupMembers.Count; i++)
                {
                    IGroupable _groupable = FindComponent<IGroupable>(groupMembers[i]);
                    _groupable.grpMemberIndex--;
                }
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

            private void SwitchLeftSelectionMode(InputAction.CallbackContext _ctx)
            {
                if (leftGrpMkgToolsData.isVolumetric)
                {
                    leftGrpMkgToolsData.isVolumetric = false;
                    leftGrpMkgToolsData.volumetricSelector.gameObject.SetActive(false);
                    leftGrpMkgToolsData.discreteSelector.SetActive(true);
                }
                else
                {
                    leftGrpMkgToolsData.isVolumetric = true;
                    leftGrpMkgToolsData.discreteSelector.SetActive(false);
                    leftGrpMkgToolsData.volumetricSelector.gameObject.SetActive(true);
                    leftGrpMkgToolsData.volumetricSelector.ResetTransform();
                }
            }

            private void SwitchRightSelectionMode(InputAction.CallbackContext _ctx)
            {
                if (rightGrpMkgToolsData.isVolumetric)
                {
                    rightGrpMkgToolsData.isVolumetric = false;
                    rightGrpMkgToolsData.volumetricSelector.gameObject.SetActive(false);
                    rightGrpMkgToolsData.discreteSelector.SetActive(true);
                }
                else
                {
                    rightGrpMkgToolsData.isVolumetric = true;
                    rightGrpMkgToolsData.discreteSelector.SetActive(false);
                    rightGrpMkgToolsData.volumetricSelector.gameObject.SetActive(true);
                    rightGrpMkgToolsData.volumetricSelector.ResetTransform();
                }
            }

            private void TryAddMemberEnd(GroupMakingToolsData _grpMkgToolsData)
            {
                if (_grpMkgToolsData.isVolumetric)
                {
                    _grpMkgToolsData.volumetricSelector.ManageActive(false);
                }
            }
            
            private void TryAddMemberStart(GroupMakingToolsData _grpMkgToolsData)
            {
                if (_grpMkgToolsData.isVolumetric)
                {
                    _grpMkgToolsData.volumetricSelector.ManageActive(true);
                }

                else
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(_grpMkgToolsData.discreteSelector.transform.position,
                                        _grpMkgToolsData.discreteSelector.transform.forward,
                                        out hitInfo, _grpMkgToolsData.maxDistance))
                    {
                        CheckCollision(hitInfo.collider.gameObject);
                    }
                }
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
                        value = refGroupNameInputField.text,
                        color = Random.ColorHSV(),
                        members = highlitables
                    },
                    0);

                //Reset group members list
                ResetGroupMemberList();

                //Hide the UI dialogue
                refUICanvas.SetActive(false);
            }
        }
    }
}

