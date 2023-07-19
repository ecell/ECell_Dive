using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

using ECellDive.Interfaces;
using ECellDive.Utility;

namespace ECellDive.Portal
{
    public class PortalManager : MonoBehaviour, IHighlightable, IFocus
    {
        #region - Highlightable Members - 
        private bool m_forceHighlight = false;
        public bool forceHighlight
        {
            get => m_forceHighlight;
            set => m_forceHighlight = value;
        }
        #endregion

        public bool hideOnStart;

        private bool isHighlighted = false;
        public Vector3 baseScale = Vector3.one;
        public Vector3 basePosition = Vector3.one;
        [Range(1, 2)] public float highlightScaleFactor = 1.25f;
        private ParticleSystem refParticleSystem;
        private ParticleSystem.EmissionModule emissionModule;

        public Color defaultPortalColor;
        public Color defaultOutlineColor;
        [SerializeField] private Renderer[] renderers;
        private MaterialPropertyBlock mpb;
        private int colorID;

        [SerializeField] private LeftRightData<InputActionReference> m_diveActions;
        public LeftRightData<InputActionReference> diveActions
        {
            get => m_diveActions;
            set => m_diveActions = value;
        }

        private bool m_isFocused;
        public bool isFocused
        {
            get => m_isFocused;
            set => m_isFocused = value;
        }

        private LeftRightData<bool> diveActionPressed;

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

            refParticleSystem = GetComponentInChildren<ParticleSystem>();
            emissionModule = refParticleSystem.emission;

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

