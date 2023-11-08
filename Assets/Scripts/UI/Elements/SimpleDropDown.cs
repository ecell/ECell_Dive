using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ECellDive.Interfaces;

namespace ECellDive.UI
{
	/// <summary>
	/// Manages a GUI element behaving like a drop down.
	/// </summary>
	public class SimpleDropDown : MonoBehaviour,
									IDropDown
	{
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

