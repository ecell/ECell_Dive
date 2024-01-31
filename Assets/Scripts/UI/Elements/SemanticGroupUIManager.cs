using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Utility;//to avoid ambiguity with UnityEngine.UI.Toggle
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.UI
{
	/// <summary>
	/// Manages the GUI container (drop down) storing every GUI element representing
	/// groups.
	/// </summary>
	public class SemanticGroupUIManager : MonoBehaviour, IDropDown
	{
		/// <summary>
		/// The reference to a toggle to activate or deactivate every UI element
		/// stored in the container.
		/// </summary>
		public Toggle refToggle;

		/// <summary>
		/// The reference to the text mesh displaying the name of the group.
		/// </summary>
		public TMP_Text refNameText;

		/// <summary>
		/// A unity event to be invoked when the container is destroyed.
		/// </summary>
		public UnityEvent OnDestroy;

		#region - IDropDown Members -
		/// <summary>
		/// The field for the <see cref="refDropDownImageCollapsed"/> property.
		/// </summary>
		[SerializeField]
		private GameObject m_refDropDownImageCollapsed;

		/// <inheritdoc/>
		public GameObject refDropDownImageCollapsed
		{
			get => m_refDropDownImageCollapsed;
			set => m_refDropDownImageCollapsed = value;
		}

		/// <summary>
		/// The field for the <see cref="refDropDownImageExpanded"/> property.
		/// </summary>
		[SerializeField]
		private GameObject m_refDropDownImageExpanded;

		/// <inheritdoc/>
		public GameObject refDropDownImageExpanded
		{
			get => m_refDropDownImageExpanded;
			set => m_refDropDownImageExpanded = value;
		}

		/// <summary>
		/// The field for the <see cref="isExpanded"/> property.
		/// </summary>
		private bool m_isExpanded = false;

		/// <inheritdoc/>
		public bool isExpanded
		{
			get => m_isExpanded;
			protected set => m_isExpanded = value;
		}

		/// <summary>
		/// The field for the <see cref="content"/> property.
		/// </summary>
		private GameObject m_content;

		/// <inheritdoc/>
		public GameObject content
		{
			get => m_content;
			set => m_content = value;
		}

		/// <summary>
		/// The field for the <see cref="scrollListHolderPrefab"/> property.
		/// </summary>
		[SerializeField] private GameObject m_scrollListHolderPrefab;

		/// <inheritdoc/>
		public GameObject scrollListHolderPrefab
		{
			get => m_scrollListHolderPrefab;
			set => m_scrollListHolderPrefab = value;
		}

		/// <summary>
		/// The field for the <see cref="scrollListPrefab"/> property.
		/// </summary>
		[SerializeField] private GameObject m_scrollListPrefab;

		/// <inheritdoc/>
		public GameObject scrollListPrefab
		{
			get => m_scrollListPrefab;
			set => m_scrollListPrefab = value;
		}

		/// <summary>
		/// The field for the <see cref="scrollListHolder"/> property.
		/// </summary>
		private GameObject m_scrollListHolder;

		/// <inheritdoc/>
		public GameObject scrollListHolder
		{
			get => m_scrollListHolder;
			set => m_scrollListHolder = value;
		}

		/// <summary>
		/// The field for the <see cref="scrollList"/> property.
		/// </summary>
		private OptimizedVertScrollList m_scrollList;

		/// <inheritdoc/>
		public OptimizedVertScrollList scrollList
		{
			get => m_scrollList;
			set => m_scrollList = value;
		}
		#endregion

		/// <summary>
		/// The procedure when destroying this semantic group.
		/// Starts the coroutine <see cref="DestroySelfC"/>.
		/// </summary>
		public void DestroySelf()
		{
			StartCoroutine(DestroySelfC());
		}
		
		/// <summary>
		/// The coroutine to destroy this semantic group.
		/// </summary>
		private IEnumerator DestroySelfC()
		{
			//Resetting the group info (color) of every child.
			foreach (RectTransform _child in scrollList.refContent)
			{
				GroupUIManager refGM = _child.gameObject.GetComponent<GroupUIManager>();
                refGM.ForceDistributeColor(false);
                yield return new WaitUntil(() => StaticReferencer.Instance.refGroupsMenu.colorBatchDistributed);
                StaticReferencer.Instance.refGroupsMenu.colorBatchDistributed = false;
			}

			//Destroying the scroll list of the content of the drop down (semantic group). 
			Destroy(scrollListHolder);

			//Hiding the object.
			gameObject.SetActive(false);

			//Remove the object from the scroll list containing every semantic group.
			transform.parent = null;

			//Invoking external functions.
			OnDestroy?.Invoke();

			//Finally, destroying the object.
			Destroy(gameObject);

			yield return null;
		}

		/// <summary>
		/// To be called back when the user wants to expand of collapse the
		/// view of the groups stored in the container.
		/// </summary>
		public void ManageExpansion()
		{
			isExpanded = !isExpanded;
			if (isExpanded)
			{
				refDropDownImageCollapsed.SetActive(false);
				refDropDownImageExpanded.SetActive(true);
				DisplayContent();
			}
			else
			{
				refDropDownImageExpanded.SetActive(false);
				refDropDownImageCollapsed.SetActive(true);
				HideContent();
			}
		}

		/// <summary>
		/// To be called back when the user wants to forcefully activate or deactivate
		/// every group stored in the container. It starts the coroutine <see cref=
		/// "OnToggleValueChangeC"/>.
		/// </summary>
		public void OnToggleValueChange()
		{
			StartCoroutine(OnToggleValueChangeC());
        }

		/// <summary>
		/// the coroutine to forcefully activate or deactivate every group stored
		/// in the container but waits that the color distribution is done among a
		/// group before moving to the next one. This is necessary to avoid saturation
		/// of the network when the color of groups are actually network variables.
		/// </summary>
		public IEnumerator OnToggleValueChangeC()
		{
			foreach(RectTransform _child in scrollList.refContent)
			{
				GroupUIManager refGM = _child.gameObject.GetComponent<GroupUIManager>();
				refGM.ForceDistributeColor(refToggle.isOn);
				yield return new WaitUntil(() => StaticReferencer.Instance.refGroupsMenu.colorBatchDistributed);
                StaticReferencer.Instance.refGroupsMenu.colorBatchDistributed = false;
                refGM.refToggle.interactable = refToggle.isOn;
			}
		}

		#region - IDropDown Methods -
		/// <inheritdoc/>
		public GameObject AddItem()
		{
			return scrollList.AddItem();
		}

		/// <inheritdoc/>
		public void DisplayContent()
		{
			m_content.SetActive(true);
			m_scrollListHolder.GetComponent<IPopUp>().PopUp();
		}

		/// <inheritdoc/>
		public void HideContent()
		{
			m_content.SetActive(false);
		}

		/// <inheritdoc/>
		public void InstantiateContent()
		{
			m_scrollListHolder = Instantiate(m_scrollListHolderPrefab, Vector3.zero, Quaternion.identity);
			m_scrollListHolder.SetActive(true);
			m_scrollListHolder.GetComponent<IPopUp>().PopUp();
			m_content = Instantiate(m_scrollListPrefab, m_scrollListHolder.transform);
			m_scrollListHolder.GetComponent<XRGrabInteractable>().colliders.Add(m_content.GetComponentInChildren<BoxCollider>());
			m_scrollListHolder.GetComponent<XRGrabInteractable>().enabled = true;

			m_scrollList = m_content.GetComponentInChildren<OptimizedVertScrollList>();
		}
		#endregion
	}
}
