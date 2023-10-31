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
							IInfoTags,
							INamed
	{
		/// <summary>
		/// The array of renderers this module might have.
		/// If nothing is set in the Editor (length = 0), the module
		/// will try to find one in this gameobject and its children
		/// in the Awake method. If really nothing is found, the array
		/// is set to null. So, if you need to know if the module has
		/// any renderer, check against null.
		/// 
		/// If you specify a renderer in the Editor, you need to specify
		/// the corresponding color property name in <see cref="renderersColorPropertyNames"/>.
		/// </summary>
		[Header("Module Parameters")]
		[Tooltip("If null, tries to find one in this gameobject and its children.")]
		[SerializeField] protected Renderer[] renderers;

		/// <summary>
		/// The names of the properties of the shaders used by the renderers
		/// to manipulate the color of the module. The names are used to search
		/// for the ColorID of the properties in the shaders which, in turn, are
		/// used to assign the color of the module in the material property block
		/// (see <see cref="mpb"/>).
		/// 
		/// In case nothing is set in the Editor <see cref="renderers"/> AND 
		/// the module automatically finds a renderer in this gameobject or its
		/// children AND nothing is set in the Editor <see cref="renderersColorPropertyNames"/>,
		/// it will default to "_BaseColor".
		/// </summary>
		[SerializeField] protected string[] renderersColorPropertyNames;

		/// <summary>
		/// The ID of the color property in the shader of the module to change its color
		/// in the material property block.
		/// </summary>
		protected int[] renderersColorPropertyIDs;

		/// <summary>
		/// The array of line renderers this module might have.
		/// If nothing is set in the Editor (length = 0), the module
		/// will try to find one in this gameobject and its children
		/// in the Awake method. If really nothing is found, the array
		/// is set to null. So, if you need to know if the module has
		/// any line renderer, check against null.
		/// 
		/// If you specify a line renderer in the Editor, you need to specify
		/// the corresponding color property name in <see cref="renderersColorPropertyNames"/>.
		/// </summary>
		[Tooltip("If null, tries to find one in this gameobject and its children.")]
		[SerializeField] protected LineRenderer[] lineRenderers;

		/// <summary>
		/// The names of the properties of the shaders used by the line renderers
		/// to manipulate the color of the module. The names are used to search
		/// for the ColorID of the properties in the shaders which, in turn, are
		/// used to assign the color of the module in the material property block
		/// (see <see cref="mpb"/>).
		/// 
		/// In case nothing is set in the Editor <see cref="lineRenderers"/> AND 
		/// the module automatically finds a line renderer in this gameobject or its
		/// children AND nothing is set in the Editor <see cref="lineRenderersColorPropertyNames"/>,
		/// it will default to "_BaseColor".
		/// </summary>
		[SerializeField] protected string[] lineRenderersColorPropertyNames;

		/// <summary>
		/// The ID of the color property in the shader of the module to change its color
		/// in the material property block.
		/// </summary>
		protected int[] lineRenderersColorPropertyIDs;

		/// <summary>
		/// The material property block used to change the color of the module while 
		/// avoiding to create a new material instance.
		/// </summary>
		protected MaterialPropertyBlock mpb;

		#region - INamed Members -
		/// <summary>
		/// The field for the property <see cref="nameField"/>
		/// </summary>
		[Header("INamed Parameters")]
		[SerializeField] TextMeshProUGUI m_nameField;

		/// <inheritdoc/>
		public TextMeshProUGUI nameField
		{
			get => m_nameField;
		}

		/// <summary>
		/// The field for the property <see cref="nameTextFieldContainer"/>
		/// </summary>
		[SerializeField] GameObject m_nameTextFieldContainer;

		/// <inheritdoc/>
		public GameObject nameTextFieldContainer
		{
			get => m_nameTextFieldContainer;
		}
		#endregion

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
		[Header("IGroupable Parameters")]
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

		#region - IColorHighlightable Members - 
		/// <summary>
		/// Field of the property <see cref="defaultColor"/>
		/// </summary>
		[Header("IColorHighlightable Parameters")]
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
		[Header("IInfoTags Parameters")]
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

			mpb = new MaterialPropertyBlock();
			
			//We try to assign default values to the renderers and the corresponding
			//color property names if they have not been set in the editor.
			if (renderers.Length == 0)
			{
				Renderer renderer = GetComponentInChildren<Renderer>();
				if (renderer != null)
				{
					renderers = new Renderer[] { renderer };
					if (renderersColorPropertyNames.Length == 0)
					{
						renderersColorPropertyNames = new string[] { "_BaseColor" };
					}
				}
				else
				{
					renderersColorPropertyNames = null;
					renderers = null;
				}
			}

			//We try to assign default values to the line renderers and the corresponding
			//color property names if they have not been set in the editor.
			if (lineRenderers.Length == 0)
			{
				LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();
				if (lineRenderer != null)
				{
					lineRenderers = new LineRenderer[] { lineRenderer };
					if (lineRenderersColorPropertyNames.Length == 0)
					{
						lineRenderersColorPropertyNames = new string[] { "_BaseColor" };
					}
				}
				else
				{
					lineRenderersColorPropertyNames = null;
					lineRenderers = null;
				}
			}

			//If we found renderers, the above code either assigned default color property
			//names or the user did it in the editor. In both cases, we the names to IDs.
			if (renderers != null)
			{
				renderersColorPropertyIDs = new int[renderersColorPropertyNames.Length];
				for (int i = 0; i < renderersColorPropertyNames.Length; i++)
				{
					renderersColorPropertyIDs[i] = Shader.PropertyToID(renderersColorPropertyNames[i]);
				}
			}
			else
			{
				renderersColorPropertyIDs = null;
			}

			//If we found line renderers, the above code either assigned default color property
			//names or the user did it in the editor. In both cases, we the names to IDs.
			if (lineRenderers != null)
			{
				lineRenderersColorPropertyIDs = new int[lineRenderersColorPropertyNames.Length];
				for (int i = 0; i < lineRenderersColorPropertyNames.Length; i++)
				{
					lineRenderersColorPropertyIDs[i] = Shader.PropertyToID(lineRenderersColorPropertyNames[i]);
				}
			}
			else
			{
				lineRenderersColorPropertyIDs = null;
			}			
			
			m_displayInfoTagsActions.left.action.performed += ManageInfoTagsDisplay;
			m_displayInfoTagsActions.right.action.performed += ManageInfoTagsDisplay;
		}

		public virtual void OnDestroy()
		{
			m_displayInfoTagsActions.left.action.performed -= ManageInfoTagsDisplay;
			m_displayInfoTagsActions.right.action.performed -= ManageInfoTagsDisplay;
		}

		private void OnEnable()
		{
			ApplyColor(defaultColor);
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
		public virtual void ApplyColor(Color _color)
		{
			if (renderers != null)
			{
				for(int i = 0; i<renderers.Length; i++)
				{
					mpb.SetVector(renderersColorPropertyIDs[i], _color);
					renderers[i].SetPropertyBlock(mpb);
				}
			}

			if (lineRenderers != null)
			{
				for (int i = 0; i < lineRenderers.Length; i++)
				{
					mpb.SetVector(lineRenderersColorPropertyIDs[i], _color);
					lineRenderers[i].SetPropertyBlock(mpb);
				}
			}
		}

		/// <inheritdoc/>
		public virtual void SetHighlight()
		{
			ApplyColor(highlightColor);
		}

		/// <inheritdoc/>
		public virtual void UnsetHighlight()
		{
			if (!m_forceHighlight)
			{
				ApplyColor(defaultColor);
			}
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

		#region - INamed Methods -
		/// <inheritdoc/>
		public void DisplayName()
		{
			nameTextFieldContainer.SetActive(true);
		}

		/// <inheritdoc/>
		public string GetName()
		{
			return nameField.text;
		}

		/// <inheritdoc/>
		public void HideName()
		{
			nameTextFieldContainer.SetActive(false);
		}

		/// <inheritdoc/>
		public void SetName(string _name)
		{
			nameField.text = _name;
		}

		/// <inheritdoc/>
		public void ShowName()
		{
			nameTextFieldContainer.GetComponent<ILookAt>().LookAt();
		}
		#endregion
	}
}
