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

        /// <summary>
        /// The logic to handle custom groups made by the user with
        /// either the discrete or volumetric selectors.
        /// </summary>
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

            /// <summary>
            /// Adds the gameobject <paramref name="_go"/> to the group.
            /// </summary>
            /// <param name="_go">The gameobject to add.</param>
            /// <param name="_goGroupInfo">The gameobject's IGroupable component.</param>
            private void AddMemberToGroup(GameObject _go, IGroupable _goGroupInfo)
            {
                _goGroupInfo.grpMemberIndex = groupMembers.Count;
                groupMembers.Add(_go);
            }

            /// <summary>
            /// Method to call back when cancelling the creation of a group.
            /// </summary>
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

            /// <summary>
            /// Tests if the gameobject <paramref name="_go"/> that collided
            /// with the selector being used is part of the target layer and
            /// proceeds if it is.
            /// </summary>
            /// <param name="_go">The gameobject that collided with the discrete
            /// or volumetric selector.</param>
            public void CheckCollision(GameObject _go)
            {
                if (IsObjectInTargetLayer(_go))
                {
                    ManageGroupMembers(_go);
                }
            }

            /// <summary>
            /// A utility function that tries to find a component in itself, its children
            /// gameobject and its parent (if it exists)
            /// </summary>
            /// <typeparam name="T">The type of the component to look for.</typeparam>
            /// <param name="_go">The source gameobject of the search.</param>
            /// <returns>The component if it found one or null if it didn't.</returns>
            /// <remarks>Implemented to handle the case when the gameobject that has
            /// the collider is not the gameobject that has the graphics renderer or any
            /// other component of interest.</remarks>
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

            /// <summary>
            /// A utility method to check if the gameobject <paramref name="_go"/> is
            /// part of the <seealso cref="GroupMakingToolsData.targetLayer"/>.
            /// </summary>
            /// <param name="_go">The gamobject to be tested.</param>
            /// <returns>True if it is in the target layer; false otherwise.</returns>
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

            /// <summary>
            /// The logic to add or remove the gameobject <paramref name="_go"/> from the
            /// group. It is added if it's not in the group and removed it is already in it.
            /// </summary>
            /// <param name="_go">The gameobject to add or remove.</param>
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

            /// <summary>
            /// Manages the visibility of the gameobject <see cref="refUICanvas"/>
            /// containing the canvas of the UI used to validate or cancel the
            /// creation of the group.
            /// </summary>
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

            /// <summary>
            /// The logic to remove the gameobject <paramref name="_go"/> from the
            /// group.
            /// </summary>
            /// <param name="_go">The gameobject to remove.</param>
            /// <param name="_goGroupInfo">The gameobject's IGroupable component.</param>
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

            /// <summary>
            /// Resets the group data.
            /// </summary>
            private void ResetGroupMemberList()
            {
                foreach(GameObject _go in groupMembers)
                {
                    IGroupable groupable = FindComponent<IGroupable>(_go);
                    groupable.grpMemberIndex = -1;
                }

                groupMembers.Clear();
            }

            /// <summary>
            /// The logic to switch between the discrete and volumetric selection
            /// mode for the left controller.
            /// </summary>
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

            /// <summary>
            /// The logic to switch between the discrete and volumetric selection
            /// mode for the right controller.
            /// </summary>
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

            /// <summary>
            /// The logic to deactivate the current selector when the
            /// selection button is released.
            /// </summary>
            /// <param name="_grpMkgToolsData">The data telling us which
            /// controller and selector is currently used. See the
            /// <see cref="GroupMakingToolsData"/> struct,
            /// <see cref="leftGrpMkgToolsData"/> and <see cref="rightGrpMkgToolsData"/>.</param>
            private void TryAddMemberEnd(GroupMakingToolsData _grpMkgToolsData)
            {
                if (_grpMkgToolsData.isVolumetric)
                {
                    _grpMkgToolsData.volumetricSelector.ManageActive(false);
                }
            }

            /// <summary>
            /// The logic to activate the current selector when the selection
            /// button has just started being pressed.
            /// </summary>
            /// <param name="_grpMkgToolsData">The data telling us which
            /// controller and selector is currently used. See the
            /// <see cref="GroupMakingToolsData"/> struct,
            /// <see cref="leftGrpMkgToolsData"/> and <see cref="rightGrpMkgToolsData"/>.</param>
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

            /// <summary>
            /// The method to callback when validating the creation of the group.
            /// </summary>
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

