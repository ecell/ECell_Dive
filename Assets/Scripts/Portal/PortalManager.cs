using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using ECellDive.Interfaces;
using ECellDive.Utility;

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
        private IDive refDivableData;

        private void Awake()
        {
            diveActions.left.action.performed += TryDiveIn;
            diveActions.right.action.performed += TryDiveIn;

            refParticleSystem = GetComponentInChildren<ParticleSystem>();
            emissionModule = refParticleSystem.emission;

            refDivableData = GetComponentInParent<IDive>();

            transform.localPosition = basePosition;

            if (hideOnStart)
            {
                gameObject.SetActive(false);
            }
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

        private void TryDiveIn(InputAction.CallbackContext _ctx)
        {
            StartCoroutine(TryDiveInC());
        }

        private IEnumerator TryDiveInC()
        {
            refDivableData.TryDiveIn();
            AnimationLoopWrapper alw = GetComponent<AnimationLoopWrapper>();
            alw.PlayLoop("PortalDive");
            yield return new WaitWhile(() => refDivableData.isDiving);
            alw.StopLoop();

        }

        #region - IHighlightable Methods -
        public void SetHighlight()
        {
            if (gameObject.activeSelf)
            {
                isHighlighted = true;
                StartCoroutine(ScaleUpC());
            }
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
    }
}

