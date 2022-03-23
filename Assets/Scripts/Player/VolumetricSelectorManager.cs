using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace UserActions
    {
        /// <summary>
        /// The logic to control a collider that can be used to perform volumetric
        /// selection of objects in the scene. It can be moved forward
        /// or backward and scaled up or down. The objects that collide with it
        /// are sent to the <seealso cref="GroupMakingManager"/>
        /// </summary>
        public class VolumetricSelectorManager : MonoBehaviour, IHighlightable
        {
            #region - IHighlightable Members - 

            [SerializeField] private Color m_defaultColor;
            public Color defaultColor
            {
                get => m_defaultColor;
                set => SetDefaultColor(value);
            }

            [SerializeField] private Color m_highlightColor;
            public Color highlightColor
            {
                get => m_highlightColor;
                set => SetHighlightColor(value);
            }

            private bool m_forceHighlight = false;
            public bool forceHighlight
            {
                get => m_forceHighlight;
                set => m_forceHighlight = value;
            }

            #endregion

            public GroupsMakingManager refGrpMkgManager;

            [Header("Distance and Scale Control Parameters")]
            public InputActionReference distanceAndScaleAction;
            [Range(0,2)] public float movementSpeed;
            private Vector3 mvtVelocity = Vector3.zero;
            [Range(0,2)] public float growthSpeed;
            private Vector3 growthVelocity = Vector3.zero;

            private SphereCollider refSphereCollider;

            private Renderer refRenderer;
            private MaterialPropertyBlock mpb;
            private int colorID;

            private void Awake()
            {
                distanceAndScaleAction.action.performed += DistanceAndScale;
            }

            private void Start()
            {
                refSphereCollider = GetComponent<SphereCollider>();
            }

            private void OnEnable()
            {
                refRenderer = GetComponentInChildren<Renderer>();
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                mpb.SetVector(colorID, defaultColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            /// <summary>
            /// Collision Event with objects in the scene.
            /// </summary>
            private void OnTriggerEnter(Collider collider)
            {
                refGrpMkgManager.CheckCollision(collider.gameObject);
            }

            /// <summary>
            /// Manages the data from the Joystick and processes it to
            /// move the gameobject and scale it.
            /// </summary>
            public void DistanceAndScale(InputAction.CallbackContext _ctx)
            {
                Vector2 _das = _ctx.ReadValue<Vector2>();
                ManageDistance(MinClamping(_das.y));
                ManageScale(MinClamping(_das.x));
            }

            /// <summary>
            /// Controls whether the volumetric selector is activated.
            /// </summary>
            /// <param name="_active">If True, the collider is enabled and
            /// the selector highlighted. If False, the collider is disabled
            /// and the selector has a default color.</param>
            public void ManageActive(bool _active)
            {
                refSphereCollider.enabled = _active;
                if (_active)
                {
                    SetHighlight();
                }
                else
                {
                    UnsetHighlight();
                }
            }

            /// <summary>
            /// Moves the selector forward or backward.
            /// </summary>
            /// <param name="_mvtFactor">If greater than 0 then forward movement;
            /// If lower than 0 then backward movement.</param>
            private void ManageDistance(float _mvtFactor)
            {
                Vector3 target = transform.position + _mvtFactor * movementSpeed * transform.forward;
                transform.position = Vector3.SmoothDamp(
                                            transform.position,
                                            target,
                                            ref mvtVelocity,
                                            0.1f);
            }

            /// <summary>
            /// Scales the selector up or down.
            /// </summary>
            /// <param name="_mvtFactor">If greater than 0 then scales up;
            /// If lower than 0 then scales down.</param>
            private void ManageScale(float _growthFactor)
            {
                Vector3 target = transform.localScale + _growthFactor * growthSpeed * Vector3.one;
                transform.localScale = Vector3.SmoothDamp(
                                                transform.localScale,
                                                target,
                                                ref growthVelocity,
                                                0.1f);
            }

            /// <summary>
            /// A hard coded method returning 0 if Abs(<paramref name="_value"/>)
            /// lower than 0.5f. Returns <paramref name="_value"/> otherwise.
            /// </summary>
            /// <remarks>Used to clamp Joystick input data since I couldn't
            /// make the built-in deadzone work as intended.</remarks>
            private float MinClamping(float _value)
            {
                if (Mathf.Abs(_value) < 0.5f)
                {
                    return 0;
                }
                else
                {
                    return _value;
                }
            }

            /// <summary>
            /// Resets the gameobject trasnforms to start position and scale.
            /// </summary>
            public void ResetTransform()
            {
                transform.localPosition = new Vector3(0, 0, 0.1f);
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            #region - IHighlightable -

            public virtual void SetDefaultColor(Color _c)
            {
                m_defaultColor = _c;
            }

            public virtual void SetHighlightColor(Color _c)
            {
                m_highlightColor = _c;
            }

            public void SetHighlight()
            {
                mpb.SetVector(colorID, highlightColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            public void UnsetHighlight()
            {
                if (!forceHighlight)
                {
                    mpb.SetVector(colorID, defaultColor);
                    refRenderer.SetPropertyBlock(mpb);
                }
            }
            #endregion
        }
    }
}

