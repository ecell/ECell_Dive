using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Interfaces;
using ECellDive.UI;
using ECellDive.Utility;
using ECellDive.Utility.Data;
using ECellDive.Utility.Data.UI;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.PlayerComponents
{
	[System.Serializable]
	public struct GroupMakingToolsData
	{
		public int targetLayer;
		public float maxDistance;
	}

	/// <summary>
	/// The logic to handle custom groups made by the user with
	/// either the discrete or volumetric selectors.
	/// </summary>
	public class GroupsMakingManager : NetworkBehaviour
	{
		public LeftRightData<InputActionReference> selection;
		public LeftRightData<InputActionReference> switchSelectionMode;

		public LeftRightData<GameObject> discreteSelector;
		public LeftRightData<VolumetricSelectorManager> volumetricSelector;

		public LeftRightData<SurgeAndShrinkInfoTag> inputModeTags;

		public GroupMakingToolsData grpMkgToolsData;

		private LeftRightData<NetworkVariable<bool>> isVolumetric = new LeftRightData<NetworkVariable<bool>>
		{
			left = new NetworkVariable<bool>(false),
			right = new NetworkVariable<bool>(false)
		};

		public List<GameObject> groupMembers = new List<GameObject>();

		public override void OnNetworkSpawn()
		{
			volumetricSelector.left.gameObject.GetComponent<TriggerBroadcaster>().onTriggerEnter += CheckCollision;
			volumetricSelector.right.gameObject.GetComponent<TriggerBroadcaster>().onTriggerEnter += CheckCollision;

			selection.left.action.started += TryAddMemberStartLeft;
			selection.left.action.canceled += TryAddMemberEndLeft;
			selection.right.action.started += TryAddMemberStartRight;
			selection.right.action.canceled += TryAddMemberEndRight;

			switchSelectionMode.left.action.performed += SwitchSelectionModeLeft;
			switchSelectionMode.right.action.performed += SwitchSelectionModeRight;

			isVolumetric.left.OnValueChanged += ApplySwitchSelectionModeLeft;
			isVolumetric.right.OnValueChanged += ApplySwitchSelectionModeRight;
		}

		public override void OnNetworkDespawn()
		{
			volumetricSelector.left.gameObject.GetComponent<TriggerBroadcaster>().onTriggerEnter -= CheckCollision;
			volumetricSelector.right.gameObject.GetComponent<TriggerBroadcaster>().onTriggerEnter -= CheckCollision;

			selection.left.action.started -= TryAddMemberStartLeft;
			selection.left.action.canceled -= TryAddMemberEndLeft;
			selection.right.action.started -= TryAddMemberStartRight;
			selection.right.action.canceled -= TryAddMemberEndRight;

			switchSelectionMode.left.action.performed -= SwitchSelectionModeLeft;
			switchSelectionMode.right.action.performed -= SwitchSelectionModeRight;

			isVolumetric.left.OnValueChanged -= ApplySwitchSelectionModeLeft;
			isVolumetric.right.OnValueChanged -= ApplySwitchSelectionModeRight;

		}

		/// <summary>
		/// Adds the gameobject <paramref name="_go"/> to the group.
		/// </summary>
		/// <param name="_go">The gameobject to add.</param>
		/// <param name="_goGroupInfo">The gameobject's IGroupable component.</param>
		private void AddMemberToGroup(GameObject _go, IGroupable _goGroupInfo)
		{
			_goGroupInfo.grpMemberIndex = groupMembers.Count;
			groupMembers.Add(_goGroupInfo.delegateTarget);
		}

		/// <summary>
		/// The logic to switch between the discrete and volumetric selection
		/// mode for the left controller.
		/// </summary>
		private void ApplySwitchSelectionModeLeft(bool _past, bool _current)
		{
			if (isVolumetric.left.Value)
			{
				discreteSelector.left.SetActive(false);
				volumetricSelector.left.gameObject.SetActive(true);
				volumetricSelector.left.ResetTransform();
				inputModeTags.left.SurgeAndShrink("Group Selector:\nVolumetric");
			}
			else
			{
				volumetricSelector.left.gameObject.SetActive(false);
				discreteSelector.left.SetActive(true);
				inputModeTags.left.SurgeAndShrink("Group Selector:\nDiscrete");
			}
		}

		/// <summary>
		/// The logic to switch between the discrete and volumetric selection
		/// mode for the left controller.
		/// </summary>
		private void ApplySwitchSelectionModeRight(bool _past, bool _current)
		{
			if (isVolumetric.right.Value)
			{
				discreteSelector.right.SetActive(false);
				volumetricSelector.right.gameObject.SetActive(true);
				volumetricSelector.right.ResetTransform();
				inputModeTags.right.SurgeAndShrink("Group Selector:\nVolumetric");
			}
			else
			{
				volumetricSelector.right.gameObject.SetActive(false);
				discreteSelector.right.SetActive(true);
				inputModeTags.right.SurgeAndShrink("Group Selector:\nDiscrete");
			}
		}

		[ServerRpc]
		public void BroadcastSelectionModeLeftServerRpc()
		{
			isVolumetric.left.Value = !isVolumetric.left.Value;
		}

		[ServerRpc]
		public void BroadcastSelectionModeRightServerRpc()
		{
			isVolumetric.right.Value = !isVolumetric.right.Value;
		}

		/// <summary>
		/// Method to call back when cancelling the creation of a group.
		/// </summary>
		public void CancelGroup()
		{
			//Reset objects Highlight
			for (int i = 0; i < groupMembers.Count; i++)
			{
				IColorHighlightable highlightable = ToFind.FindComponent<IColorHighlightable>(groupMembers[i]);
				highlightable.forceHighlight = false;
				highlightable.UnsetHighlight();
			}

			//Reset group members list
			ResetGroupMemberList();
		}

		/// <summary>
		/// Tests if the gameobject <paramref name="_go"/> that collided
		/// with the selector being used is part of the target layer and
		/// proceeds if it is.
		/// </summary>
		/// <param name="_collider">The collider we want to check.</param>
		public void CheckCollision(Collider _collider)
		{
			if (IsObjectInTargetLayer(_collider.gameObject))
			{
				ManageGroupMembers(_collider.gameObject);
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
				IColorHighlightable highlightable = ToFind.FindComponent<IColorHighlightable>(_go);
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

				StaticReferencer.Instance.refGroupsMakingUIManager.ManageUIConfirmationCanvas(groupMembers.Count);
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

		private void SwitchSelectionModeLeft(InputAction.CallbackContext _ctx)
		{
			if (IsOwner)
			{
				BroadcastSelectionModeLeftServerRpc();
			}
		}

		private void SwitchSelectionModeRight(InputAction.CallbackContext _ctx)
		{
			if (IsOwner)
			{
				BroadcastSelectionModeRightServerRpc();
			}
		}

		/// <summary>
		/// The logic to deactivate the current selector when the
		/// selection button is released for the left controller.
		/// </summary>
		private void TryAddMemberEndLeft(InputAction.CallbackContext _ctx)
		{
			if (isVolumetric.left.Value)
			{
				volumetricSelector.left.ManageActive(false);
			}
		}

		/// <summary>
		/// The logic to deactivate the current selector when the
		/// selection button is released for the right controller.
		/// </summary>
		private void TryAddMemberEndRight(InputAction.CallbackContext _ctx)
		{
			if (isVolumetric.right.Value)
			{
				volumetricSelector.right.ManageActive(false);
			}
		}

		/// <summary>
		/// The logic to activate the current selector when the selection
		/// button has just started being pressed for the left controller.
		/// </summary>
		private void TryAddMemberStartLeft(InputAction.CallbackContext _ctx)
		{
			if (isVolumetric.left.Value)
			{
				volumetricSelector.left.ManageActive(true);
			}

			else
			{
				RaycastHit hitInfo;
				if (Physics.Raycast(discreteSelector.left.transform.position,
									discreteSelector.left.transform.forward,
									out hitInfo, grpMkgToolsData.maxDistance))
				{
					CheckCollision(hitInfo.collider);
				}
			}
		}

		/// <summary>
		/// The logic to activate the current selector when the selection
		/// button has just started being pressed for the right controller.
		/// </summary>
		private void TryAddMemberStartRight(InputAction.CallbackContext _ctx)
		{
			if (isVolumetric.right.Value)
			{
				volumetricSelector.right.ManageActive(true);
			}

			else
			{
				RaycastHit hitInfo;
				if (Physics.Raycast(discreteSelector.right.transform.position,
									discreteSelector.right.transform.forward,
									out hitInfo, grpMkgToolsData.maxDistance))
				{
					CheckCollision(hitInfo.collider);
				}
			}
		}

		/// <summary>
		/// The method to callback when validating the creation of the group.
		/// </summary>
		/// <param name="_groupName">
		/// The name of the group to be created.
		/// </param>
		public void ValidateGroup(string _groupName)
		{
			//Get Highlightables and reset force highlight
			IColorHighlightable[] highlitables = new IColorHighlightable[groupMembers.Count];
			for (int i = 0; i < groupMembers.Count; i++)
			{
				highlitables[i] = ToFind.FindComponent<IColorHighlightable>(groupMembers[i]);
				highlitables[i].forceHighlight = false;
			}

			StaticReferencer.Instance.refGroupsMenu.AddGroupUI(new GroupData
			{
				value = _groupName,
				color = Random.ColorHSV(),
				members = highlitables
			},
				0);

			//Reset group members list
			ResetGroupMemberList();
		}
	}
}