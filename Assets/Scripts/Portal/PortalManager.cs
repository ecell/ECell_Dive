using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using ECellDive.Interfaces;

namespace ECellDive.Portal
{
    public class PortalManager : MonoBehaviour, IHighlightable
    {
        #region - Highlightable Members - 
        private bool m_forceHighlight = false;
        public bool forceHighlight
        {
            get => m_forceHighlight;
            set => m_forceHighlight = value;
        }
        #endregion
        private bool isHighlighted = false;
        [Range(1, 5)] public float highlightEmissionFactor = 3f;
        public Vector3 baseScale = Vector3.one;
        [Range(1, 2)] public float highlightScaleFactor = 1.25f;
        private ParticleSystem refParticleSystem;
        private ParticleSystem.EmissionModule emissionModule;


        [SerializeField] private LeftRightData<InputActionReference> m_diveActions;
        public LeftRightData<InputActionReference> diveActions
        {
            get => m_diveActions;
            set => m_diveActions = value;
        }
        private IDive refDivableData;

        private void Awake()
        {
            diveActions.left.action.performed += TryDiveIn;
            diveActions.right.action.performed += TryDiveIn;

            refParticleSystem = GetComponentInChildren<ParticleSystem>();
            emissionModule = refParticleSystem.emission;

            refDivableData = GetComponentInParent<IDive>();
        }

        private void TryDiveIn(InputAction.CallbackContext _ctx)
        {
            refDivableData.TryDiveIn();
        }

        #region - IHighlightable Methods -
        public void SetHighlight()
        {
            isHighlighted = true;
            emissionModule.rateOverTimeMultiplier *= highlightEmissionFactor;
            StartCoroutine(ScaleUpC());
        }

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

        public void UnsetHighlight()
        {
            isHighlighted=false;
            if (!forceHighlight)
            {
                emissionModule.rateOverTimeMultiplier /= highlightEmissionFactor;
                StartCoroutine(ScaleDownC());
            }
        }
        #endregion
    }
}

