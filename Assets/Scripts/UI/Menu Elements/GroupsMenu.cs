using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Utility.Data.UI;

namespace ECellDive.UI
{
	/// <summary>
	/// Manages the GUI that displays every group either produced by a "GroupByModule"
	/// or made out of selected objects by the user.
	/// </summary>
	public class GroupsMenu : MonoBehaviour
	{
		/// <summary>
		/// Reference top the gameobject that will contain every list of group members.
		/// </summary>
		public GameObject refAllUIContainer;

		/// <summary>
		/// The reference to the scroll list that will contain the drop down buttons
		/// to corresponding to the groups. Pressing the drop down will display the
		/// list of members of the group.
		/// </summary>
		public OptimizedVertScrollList semanticGroupsScrollList;

		/// <summary>
		/// The list of every drop down button that will display the list of members
		/// of the groups.
		/// </summary>
		private List<IDropDown> allDropDowns = new List<IDropDown>();

		/// <summary>
		/// A boolean to hide the menu on start.
		/// </summary>
		public bool hideOnStart = true;

		/// <summary>
		/// A boolean to block the color attribution to the members of the groups.
		/// This is a workaround to the network synchronization of the color of the
		/// mudules. When groups are large, the multiple calls to the RPCs to change
		/// the color of the modules can cause a crash.
		/// </summary>
		private bool colorBatchDistributed = false;

		private void Start()
		{
			if (hideOnStart)
			{
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Adds a <see cref="GroupUIManager"/> object at the end of the 
		/// <see cref="refAllUIContainer"/>.
		/// </summary>
		/// <param name="_groupData">
		/// The data needed to correctly initialize the <see cref="GroupUIManager"/>.
		/// </param>
		/// <param name="_parent">
		/// The reference to the drop down the new <see cref="GroupUIManager"/>
		/// should be part of.
		/// </param>
		private void AddGroupUI(GroupData _groupData, IDropDown _parent)
		{
			GameObject groupUI = _parent.AddItem();
			groupUI.SetActive(true);
			groupUI.GetComponent<GroupUIManager>().SetData(_groupData);
			groupUI.GetComponent<GroupUIManager>().ForceDistributeColor(true);
			_parent.scrollList.UpdateScrollList();
		}

		/// <summary>
		/// Adds a <see cref="GroupUIManager"/> object to the drop down with 
		/// index <paramref name="_parentIndex"/> in the list <see cref="allDropDowns"/>.
		/// </summary>
		/// <param name="_groupData">
		/// The data needed to correctly initialize the <see cref="GroupUIManager"/>.
		/// </param>
		/// <param name="_parentIndex">
		/// The index of the parent drop down in which we want to insert the newly
		/// created <see cref="GroupUIManager"/>.
		/// </param>
		/// <remarks>
		/// This is useful when we want to add a <see cref="GroupUIManager"/>
		/// to a previously created <see cref="IDropDown"/>.
		/// </remarks>
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
		/// <param name="_semanticTerm">
		/// The name of the container.
		/// </param>
		/// <param name="_groupsData">
		/// The data of every group that falls in the container.
		/// </param>
		/// <remarks>
		/// Typically usefull to manage every group produced by
		/// a "Group By" operation.
		/// </remarks>
		public IEnumerator AddSemanticTermUI(string _semanticTerm, List<GroupData> _groupsData)
		{
			//Creating the drop down button
			GameObject semanticTermUI = semanticGroupsScrollList.AddItem();
			semanticTermUI.SetActive(true);
			semanticTermUI.GetComponentInChildren<TMP_Text>().text = _semanticTerm;
			IDropDown ddComponent = semanticTermUI.GetComponent<IDropDown>();

			//Creating the scroll list menu that will contain the objects
			//controlled by the drop down.
			ddComponent.InstantiateContent();
			ddComponent.scrollListHolder.transform.SetParent(refAllUIContainer.transform);
			allDropDowns.Add(ddComponent);

			foreach (GroupData _groupData in _groupsData)
			{
				AddGroupUI(_groupData, ddComponent);
				yield return new WaitUntil(()=>colorBatchDistributed);
				colorBatchDistributed = false;
			}
			semanticGroupsScrollList.UpdateScrollList();
			semanticGroupsScrollList.UpdateAllChildrenVisibility();
		}

		/// <summary>
		/// Public interface to start the coroutine that will update the color
		/// of the members in <paramref name="_members"/> to <paramref name="_color"/>.
		/// </summary>
		/// <param name="_color">
		/// The new color to be applied to the members of the group.
		/// </param>
		/// <param name="_members">
		/// The list of members of the group.
		/// </param>
		public void DistributeColorToMembers(Color _color, IHighlightable[] _members)
		{
			gameObject.SetActive(true);
			StartCoroutine(DistributeColorToMembersC(_color, _members));
		}

		/// <summary>
		/// Internal method to update the color of the GameObjects in the group.
		/// </summary>
		/// <param name="_color">
		/// The new color to be applied to the members of the group.
		/// </param>
		/// <param name="_members">
		/// The list of members of the group.
		/// </param>
		private IEnumerator DistributeColorToMembersC(Color _color, IHighlightable[] _members)
		{
			ushort batchCounter = 0;
			foreach (IColorHighlightable _member in _members)
			{
				_member.defaultColor = _color;
				_member.highlightColor = new Color(1f - _color.r, 1f - _color.g, 1f - _color.b, 1f);
				_member.ApplyColor(_color);

				batchCounter++;

				if (batchCounter == 25)
				{
					yield return new WaitForEndOfFrame();
					batchCounter = 0;
				}
			}
			colorBatchDistributed = true;
		}

		/// <summary>
		/// Adds the default semantic groups to the list.
		/// </summary>
		public void Initialize()
		{
			if (gameObject.activeSelf)
			{
				StartCoroutine(AddSemanticTermUI("Custom Groups", new List<GroupData>()));
			}
		}
	}
}