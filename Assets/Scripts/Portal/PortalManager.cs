using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

using ECellDive.Interfaces;
using ECellDive.Utility;
using ECellDive.Utility.Data;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.Portal
{
	/// <summary>
	/// The logic behind the portals used to dive between dive scenes.
	/// </summary>
	/// <remarks>
	/// This code assumes a specific shader is used for the portal.
	/// </remarks>
	public class PortalManager : MonoBehaviour, IHighlightable, IFocus
	{
		#region - Highlightable Members - 
		/// <summary>
		/// The field for the <see cref="forceHighlight"/> property.
		/// </summary>
		private bool m_forceHighlight = false;

		/// <inheritdoc/>
		public bool forceHighlight
		{
			get => m_forceHighlight;
			set => m_forceHighlight = value;
		}
		#endregion

		#region - IFocus Members -
		/// <summary>
		/// The field for the <see cref="isFocused"/> property.
		/// </summary>
		private bool m_isFocused;

		/// <inheritdoc/>
		public bool isFocused
		{
			get => m_isFocused;
			set => m_isFocused = value;
		}
		#endregion

		/// <summary>
		/// A boolean to control whether the portal should be hidden on 
		/// first spawned.
		/// </summary>
		public bool hideOnStart;

		/// <summary>
		/// A boolean to inform whether the portal is currently highlighted.
		/// </summary>
		private bool isHighlighted = false;

		/// <summary>
		/// A vector 3 to store the base scale of the portal.
		/// </summary>
		public Vector3 baseScale = Vector3.one;

		/// <summary>
		/// A vector 3 to store the base position of the portal.
		/// </summary>
		public Vector3 basePosition = Vector3.one;

		/// <summary>
		/// A multiplier to scale the portal up when highlighted.
		/// </summary>
		[Range(1, 2)] public float highlightScaleFactor = 1.25f;

		/// <summary>
		/// The default color of the portal.
		/// </summary>
		public Color defaultPortalColor;

		/// <summary>
		/// The default color of the portal's outline.
		/// </summary>
		/// <remarks>
		/// Usefull only as long as the a specific shader is used for the portal.
		/// </remarks>
		public Color defaultOutlineColor;

		/// <summary>
		/// The list of renderers to be altered when the portal is highlighted.
		/// </summary>
		[SerializeField] private Renderer[] renderers;

		/// <summary>
		/// The material property block used to access the shader properties
		/// and alter the portal's color.
		/// </summary>
		private MaterialPropertyBlock mpb;

		/// <summary>
		/// The ID of the color property in the shader.
		/// </summary>
		private int colorID;

		/// <summary>
		/// The field for the <see cref="diveActions"/> property.
		/// </summary>
		[SerializeField] private LeftRightData<InputActionReference> m_diveActions;

		/// <summary>
		/// The reference to the left and right dive actions.
		/// </summary>
		public LeftRightData<InputActionReference> diveActions
		{
			get => m_diveActions;
			set => m_diveActions = value;
		}

		/// <summary>
		/// Booleans to know whether the dive action is pressed on the left and right
		/// controllers
		/// </summary>
		private LeftRightData<bool> diveActionPressed;

		/// <summary>
		/// The reference to the IDive interface of the module this portal is attached to.
		/// </summary>
		private IDive refDivableData;

		private void Awake()
		{
			diveActions.left.action.started += ctx => diveActionPressed.left = true;
			diveActions.left.action.started += SendHapticImpulse;
			diveActions.left.action.performed += TryDiveIn;
			diveActions.left.action.canceled += CancelHapticImpulse;

			diveActions.right.action.started += ctx => diveActionPressed.right = true;
			diveActions.right.action.started += SendHapticImpulse;
			diveActions.right.action.performed += TryDiveIn;
			diveActions.right.action.canceled += CancelHapticImpulse;

			refDivableData = GetComponentInParent<IDive>();

			transform.localPosition = basePosition;

			if (hideOnStart)
			{
				gameObject.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			diveActions.left.action.started -= ctx => diveActionPressed.left = true;
			diveActions.left.action.started -= SendHapticImpulse;
			diveActions.left.action.performed -= TryDiveIn;
			diveActions.left.action.canceled -= CancelHapticImpulse;

			diveActions.right.action.started -= ctx => diveActionPressed.right = true;
			diveActions.right.action.started -= SendHapticImpulse;
			diveActions.right.action.performed -= TryDiveIn;
			diveActions.right.action.canceled -= CancelHapticImpulse;
		}

		private void OnEnable()
		{
			mpb = new MaterialPropertyBlock();
			colorID = Shader.PropertyToID("_Color");
			mpb.SetVector(colorID, defaultPortalColor);
			renderers[0].SetPropertyBlock(mpb);//portal

			mpb.SetVector(colorID, defaultOutlineColor);
			renderers[1].SetPropertyBlock(mpb);//outline
		}

		/// <summary>
		/// Sends an "intensity-zero" haptic impulse to the controllers which effectively
		/// cancels the previous impulse, if any.
		/// </summary>
		private void CancelHapticImpulse(InputAction.CallbackContext _ctx)
		{
			if (m_isFocused)
			{
				ActionBasedController left = StaticReferencer.Instance.riControllersGO.left.GetComponent<ActionBasedController>();
				ActionBasedController right = StaticReferencer.Instance.riControllersGO.right.GetComponent<ActionBasedController>();

				if (diveActionPressed.left)
				{
					left.SendHapticImpulse(0f, 1f);
				}

				if (diveActionPressed.right)
				{
					right.SendHapticImpulse(0f, 1f);
				}
			}
			diveActionPressed.right = false;
			diveActionPressed.left = false;
		}

		/// <summary>
		/// Coroutine-based animation to smooth a scale up of the portal.
		/// </summary>
		/// <remarks>Used in SetHighlight.</remarks>
		private IEnumerator ScaleUpC()
		{
			Vector3 targetScale = highlightScaleFactor * baseScale;
			while (isHighlighted && targetScale.x - transform.localScale.x > 0.001f)
			{
				transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 10f * Time.deltaTime);
				yield return new WaitForFixedUpdate();
			}
			if (isHighlighted)
			{
				transform.localScale = targetScale;
			}
		}

		/// <summary>
		/// Coroutine-based animation to smooth a scale down of the portal.
		/// </summary>
		/// <remarks>Used in UnsetHighlight.</remarks>
		private IEnumerator ScaleDownC()
		{
			while (!isHighlighted && transform.localScale.x - baseScale.x > 0.001f)
			{
				transform.localScale = Vector3.Lerp(transform.localScale, baseScale, 10f * Time.deltaTime);
				yield return new WaitForFixedUpdate();
			}
			if (!isHighlighted)
			{
				transform.localScale = baseScale;
			}
		}

		/// <summary>
		/// Sends a haptic impulse to the controllers left or right controller.
		/// Called back when the left or right dive action is pressed.
		/// </summary>
		/// <param name="_ctx">
		/// The context of the input action at the time of the callback.
		/// It is necessary to satisfy the constraint on the signature of the callback
		/// but we are not using it.
		/// </param>
		private void SendHapticImpulse(InputAction.CallbackContext _ctx)
		{
			if (m_isFocused)
			{
				ActionBasedController left = StaticReferencer.Instance.riControllersGO.left.GetComponent<ActionBasedController>();
				ActionBasedController right = StaticReferencer.Instance.riControllersGO.right.GetComponent<ActionBasedController>();

				if (diveActionPressed.left)
				{
					left.SendHapticImpulse(0.5f, 1f);
				}

				if (diveActionPressed.right)
				{
					right.SendHapticImpulse(0.5f, 1f);
				}
			}
		}

		/// <summary>
		/// The callback function to trigger the dive of a user.
		/// </summary>
		private void TryDiveIn(InputAction.CallbackContext _ctx)
		{
			StartCoroutine(TryDiveInC());
		}

		/// <summary>
		/// The coroutine controlling the dive animation and the dive itself.
		/// </summary>
		private IEnumerator TryDiveInC()
		{
			refDivableData.TryDiveIn();
			AnimationLoopWrapper alw = GetComponent<AnimationLoopWrapper>();
			alw.PlayLoop("PortalDive");
			yield return new WaitWhile(() => refDivableData.isDiving);
			alw.StopLoop();
		}

		#region - IHighlightable Methods -
		///<inheritdoc/>
		public void SetHighlight()
		{
			if (gameObject.activeSelf)
			{
				isHighlighted = true;
				StartCoroutine(ScaleUpC());
			}
		}

		///<inheritdoc/>
		public void UnsetHighlight()
		{
			if (gameObject.activeSelf)
			{
				isHighlighted=false;
				if (!forceHighlight)
				{
					StartCoroutine(ScaleDownC());
				}
			}
		}
		#endregion

		#region - IFocus Methods -
		///<inheritdoc/>
		public void SetFocus()
		{
			m_isFocused = true;
		}

		///<inheritdoc/>
		public void UnsetFocus()
		{
			m_isFocused = false;
		}

		#endregion
	}
}

