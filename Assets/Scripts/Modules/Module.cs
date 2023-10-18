using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.UI;
using ECellDive.Utility;
using ECellDive.Utility.Data;

namespace ECellDive.Modules
{
	/// <summary>
	/// Base class holding references and methods used to manipulate
	/// the game object representation of a module.
	/// </summary>
	public class Module : MonoBehaviour,
							IFocus,
							IGroupable,
							IColorHighlightable,
							IInfoTags
	{
		/// <summary>
		/// The test mesh to display the name of the module.
		/// </summary>
		[Header("Module Info")]
		public TextMeshProUGUI refName;

		#region - IFocus Members -
		/// <summary>
		/// Field of the property <see cref="isFocused"/>
		/// </summary>
		private bool m_isFocused = false;
			
		/// <inheritdoc/>
		public bool isFocused
		{
			get => m_isFocused;
			set => isFocused = m_isFocused;
		}

		#endregion

		#region - IGroupable Members -
		/// <summary>
		/// Field of the property <see cref="grpMemberIndex"/>
		/// </summary>
		private int m_grpMemberIndex = -1;
		   
		/// <inheritdoc/>
		public int grpMemberIndex
		{
			get => m_grpMemberIndex;
			set => m_grpMemberIndex = value;
		}

		/// <summary>
		/// Field of the property <see cref="delegateTarget"/>
		/// </summary>
		[SerializeField] private GameObject m_delegateTarget;

		/// <inheritdoc/>
		public GameObject delegateTarget
		{
			get => m_delegateTarget;
			private set => m_delegateTarget = value;
		}
		#endregion

		#region - IHighlightable Members - 
		/// <summary>
		/// Field of the property <see cref="defaultColor"/>
		/// </summary>
		[SerializeField] private Color m_defaultColor;
			
		/// <inheritdoc/>
		public Color defaultColor {
			get => m_defaultColor;
			set => m_defaultColor = value;
		}

		/// <summary>
		/// Field of the property <see cref="highlightColor"/>
		/// </summary>
		[SerializeField] private Color m_highlightColor;

		/// <inheritdoc/>
		public Color highlightColor {
			get => m_highlightColor;
			set => m_highlightColor = value;
		}
			
		/// <summary>
		/// Field of the property <see cref="forceHighlight"/>
		/// </summary>
		private bool m_forceHighlight = false;

		/// <inheritdoc/>
		public bool forceHighlight
		{
			get => m_forceHighlight;
			set => m_forceHighlight = value;
		}

		#endregion

		#region - IInfoTags Members -

		/// <inheritdoc/>
		public bool areVisible { get; set; }

		/// <summary>
		/// Field of the property <see cref="displayInfoTagsActions"/>
		/// </summary>
		[Header("Info Tags Data")]
		public LeftRightData<InputActionReference> m_displayInfoTagsActions;

		/// <inheritdoc/>
		public LeftRightData<InputActionReference> displayInfoTagsActions
		{
			get => m_displayInfoTagsActions;
			set => m_displayInfoTagsActions = value;
		}

		/// <summary>
		/// Field of the property <see cref="refInfoTagPrefab"/>
		/// </summary>
		public GameObject m_refInfoTagPrefab;

		/// <inheritdoc/>
		public GameObject refInfoTagPrefab
		{
			get => m_refInfoTagPrefab;
			set => m_refInfoTagPrefab = value;
		}

		/// <summary>
		/// Field of the property <see cref="refInfoTagsContainer"/>
		/// </summary>
		public GameObject m_refInfoTagsContainer;

		/// <inheritdoc/>
		public GameObject refInfoTagsContainer
		{
			get => m_refInfoTagsContainer;
			set => m_refInfoTagsContainer = value;
		}
		#endregion

		protected virtual void Awake()
		{
			areVisible = false;

			//If the target has not been set in the editor to a specific gameobject,
			//we set it to this game object by default. 
			if (m_delegateTarget == null)
			{
				m_delegateTarget = gameObject;
			}

			m_displayInfoTagsActions.left.action.performed += ManageInfoTagsDisplay;
			m_displayInfoTagsActions.right.action.performed += ManageInfoTagsDisplay;
		}

		public virtual void OnDestroy()
		{
			m_displayInfoTagsActions.left.action.performed -= ManageInfoTagsDisplay;
			m_displayInfoTagsActions.right.action.performed -= ManageInfoTagsDisplay;
		}

		/// <summary>
		/// Input call back action on which the floating display
		/// is turned on or off.
		/// </summary>
		/// <param name="_ctx">The input action callback context</param>
		private void ManageInfoTagsDisplay(InputAction.CallbackContext _ctx)
		{
			if (isFocused)
			{
				areVisible = !areVisible;
				if (areVisible)
				{
					DisplayInfoTags();
				}
				else
				{
					HideInfoTags();
				}
			}
		}

		/// <summary>
		/// The method to call when we wish to destroy a GameNetModule.
		/// </summary>
		public void SelfDestroy()
		{
			Destroy(gameObject);
		}

		public void SetName(string _name)
		{
			refName.text = _name;
		}

		/// <summary>
		/// Makes sure the name of the module's name faces the
		/// Player's POV and is therefore readable.
		/// </summary>
		public void ShowNameToPlayer()
		{
			Positioning.UIFaceTarget(refName.gameObject.transform.parent.gameObject, Camera.main.transform);
		}

		#region - IFocus Methods -
		/// <inheritdoc/>
		public void SetFocus()
		{
			m_isFocused = true;
		}

		/// <inheritdoc/>
		public void UnsetFocus()
		{
			m_isFocused = false;
		}
		#endregion

		#region - IColorHighlightable Methods -
		/// <inheritdoc/>
		public virtual void ApplyColor(Color _color)
		{

		}

		/// <inheritdoc/>
		public virtual void SetHighlight()
		{
		}

		/// <inheritdoc/>
		public virtual void UnsetHighlight()
		{
		}
		#endregion

		#region - IInfoTags Methods-
		/// <inheritdoc/>
		public void DisplayInfoTags()
		{
			foreach (Transform _infoTag in refInfoTagsContainer.transform)
			{
				_infoTag.gameObject.SetActive(true);
			}
		}

		/// <inheritdoc/>
		public void HideInfoTags()
		{
			foreach (Transform _infoTag in refInfoTagsContainer.transform)
			{
				_infoTag.gameObject.SetActive(false);
			}
		}

		/// <inheritdoc/>
		public void InstantiateInfoTag(Vector2 _xyPosition, string _content)
		{
			GameObject infoTag = Instantiate(refInfoTagPrefab, refInfoTagsContainer.transform);
			infoTag.transform.localPosition = new Vector3(_xyPosition.x, _xyPosition.y, 0f);
			infoTag.GetComponent<InfoDisplayManager>().SetText(_content);
		}

		/// <inheritdoc/>
		public void InstantiateInfoTags(string[] _content)
		{
			float angle = 360 / _content.Length;
			float radius = 1.25f * Mathf.Max(new float[]{transform.localScale.x,
															transform.localScale.y,
															transform.localScale.z });

			for (int i = 0; i < _content.Length; i++)
			{
				Vector2 xyPosition = Positioning.RadialPosition(radius, i * angle);
				InstantiateInfoTag(xyPosition, _content[i]);
			}
		}

		/// <inheritdoc/>
		public void ShowInfoTags()
		{
			foreach (Transform _infoTag in refInfoTagsContainer.transform)
			{
				_infoTag.gameObject.GetComponent<InfoDisplayManager>().LookAt();
			}
		}
		#endregion
	}
}
