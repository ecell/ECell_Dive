using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Interfaces;
using ECellDive.UI;
using ECellDive.Utility;

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
            public int targetLayer;
            public float maxDistance;
        }

        /// <summary>
        /// The logic to handle custom groups made by the user with
        /// either the discrete or volumetric selectors.
        /// </summary>
        public class GroupsMakingManager : NetworkBehaviour
        {
            public GroupsMakingUIManager refUIManager;
            public GroupsMakingManager refSecondGrpSelector;

            public GroupMakingActionData grpMkgActionData;

            public GroupMakingToolsData grpMkgToolsData;

            private NetworkVariable<bool> isVolumetric = new NetworkVariable<bool>(false);

            public List<GameObject> groupMembers = new List<GameObject>();

            public override void OnNetworkSpawn()
            {
                grpMkgActionData.selection.action.started += TryAddMemberStart;
                grpMkgActionData.selection.action.canceled += TryAddMemberEnd;
                grpMkgActionData.switchSelectionMode.action.performed += SwitchSelectionMode;

                isVolumetric.OnValueChanged += ApplySwitchSelectionMode;
            }

            public override void OnNetworkDespawn()
            {
                grpMkgActionData.selection.action.started -= TryAddMemberStart;
                grpMkgActionData.selection.action.canceled -= TryAddMemberEnd;
                grpMkgActionData.switchSelectionMode.action.performed -= SwitchSelectionMode;

                isVolumetric.OnValueChanged -= ApplySwitchSelectionMode;

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
            /// The logic to switch between the discrete and volumetric selection
            /// mode for the left controller.
            /// </summary>
            private void ApplySwitchSelectionMode(bool _past, bool _current)
            {
                if (isVolumetric.Value)
                {
                    grpMkgToolsData.discreteSelector.SetActive(false);
                    grpMkgToolsData.volumetricSelector.gameObject.SetActive(true);
                    grpMkgToolsData.volumetricSelector.ResetTransform();
                }
                else
                {
                    grpMkgToolsData.volumetricSelector.gameObject.SetActive(false);
                    grpMkgToolsData.discreteSelector.SetActive(true);
                }
            }

            [ServerRpc]
            public void BroadcastSelectionModeServerRpc()
            {
                isVolumetric.Value = !isVolumetric.Value;
            }

            /// <summary>
            /// Method to call back when cancelling the creation of a group.
            /// </summary>
            public void CancelGroup()
            {
                //Reset objects Highlight
                for (int i = 0; i < groupMembers.Count; i++)
                {
                    IHighlightable highlitable = ToFind.FindComponent<IHighlightable>(groupMembers[i]);
                    highlitable.forceHighlight = false;
                    highlitable.UnsetHighlight();
                }

                //Reset group members list
                ResetGroupMemberList();

                //Hide the UI dialogue
                refUIManager.CloseUI();
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
            /// A utility method to check if the gameobject <paramref name="_go"/> is
            /// part of the <seealso cref="GroupMakingToolsData.targetLayer"/>.
            /// </summary>
            /// <param name="_go">The gamobject to be tested.</param>
            /// <returns>True if it is in the target layer; false otherwise.</returns>
            private bool IsObjectInTargetLayer(GameObject _go)
            {
                XRGrabInteractable interactable = ToFind.FindComponent<XRGrabInteractable>(_go);
                if (interactable != null)
                {
                    int targetLayerMask = 1 << grpMkgToolsData.targetLayer;
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
                IGroupable groupable = ToFind.FindComponent<IGroupable>(_go);
                if (groupable != null)
                {
                    IHighlightable highlightable = ToFind.FindComponent<IHighlightable>(_go);
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

                    refUIManager.ManageUIConfirmationCanvas(groupMembers.Count);
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
                groupMembers.RemoveAt(_goGroupInfo.grpMemberIndex);
                for (int i = _goGroupInfo.grpMemberIndex; i < groupMembers.Count; i++)
                {
                    IGroupable _groupable = ToFind.FindComponent<IGroupable>(groupMembers[i]);
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
                    IGroupable groupable = ToFind.FindComponent<IGroupable>(_go);
                    groupable.grpMemberIndex = -1;
                }

                groupMembers.Clear();
            }
            
            private void SwitchSelectionMode(InputAction.CallbackContext _ctx)
            {
                if (IsOwner)
                {
                BroadcastSelectionModeServerRpc();
                }
            }

            /// <summary>
            /// The logic to deactivate the current selector when the
            /// selection button is released.
            /// </summary>
            private void TryAddMemberEnd(InputAction.CallbackContext _ctx)
            {
                if (isVolumetric.Value)
                {
                    grpMkgToolsData.volumetricSelector.ManageActive(false);
                }
            }

            /// <summary>
            /// The logic to activate the current selector when the selection
            /// button has just started being pressed.
            /// </summary>
            private void TryAddMemberStart(InputAction.CallbackContext _ctx)
            {
                if (isVolumetric.Value)
                {
                    grpMkgToolsData.volumetricSelector.ManageActive(true);
                }

                else
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(grpMkgToolsData.discreteSelector.transform.position,
                                        grpMkgToolsData.discreteSelector.transform.forward,
                                        out hitInfo, grpMkgToolsData.maxDistance))
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
                List<GameObject> allGroupMembers = groupMembers;
                allGroupMembers.AddRange(refSecondGrpSelector.groupMembers);

                //Get Highlightables and reset force highlight
                IHighlightable[] highlitables = new IHighlightable[allGroupMembers.Count];
                for (int i = 0; i < allGroupMembers.Count; i++)
                {
                    highlitables[i] = ToFind.FindComponent<IHighlightable>(allGroupMembers[i]);
                    highlitables[i].forceHighlight = false;
                }

                refUIManager.NewGroupUiElement(highlitables);

                //Reset group members list
                ResetGroupMemberList();

                //Hide the UI dialogue
                refUIManager.CloseUI();
            }
        }
    }
}

