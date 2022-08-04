using Unity.Netcode;
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
        /// are sent to the <seealso cref="GroupsMakingManager"/>
        /// </summary>
        public class VolumetricSelectorManager : NetworkBehaviour,
                                                    IHighlightable
        {
            #region - IHighlightable Members - 

            [SerializeField] private NetworkVariable<Color> m_currentColor;
            public NetworkVariable<Color> currentColor
            {
                get => m_currentColor;
                set => m_currentColor = value;
            }

            [SerializeField] private Color m_defaultColor;
            public Color defaultColor
            {
                get => m_defaultColor;
                set => m_defaultColor = value;
            }

            [SerializeField] private Color m_highlightColor;
            public Color highlightColor
            {
                get => m_highlightColor;
                set => m_highlightColor = value;
            }

            private bool m_forceHighlight = false;
            public bool forceHighlight
            {
                get => m_forceHighlight;
                set => m_forceHighlight = value;
            }

            #endregion

            public GroupsMakingManager refGrpMkgManager;
            public InputActionReference distanceAndScaleAction;

            [Header("Movement Parameters")]
            public Vector3 defaultPosition;
            public float maxDistance;
            [Range(0,1)] public float movementSpeed;
            private Vector3 mvtVelocity = Vector3.zero;

            [Header("Scale Parameters")]
            public Vector3 defaultScale;
            public float minScaleFactor;
            private Vector3 minScale;
            public float maxScaleFactor;
            private Vector3 maxScale;
            [Range(0,1)] public float growthSpeed;
            private Vector3 growthVelocity = Vector3.zero;

            private SphereCollider refSphereCollider;

            private Renderer refRenderer;
            private MaterialPropertyBlock mpb;
            private int colorID;

            private NetworkVariable<bool> isActive = new NetworkVariable<bool>(default,
                default, NetworkVariableWritePermission.Owner);
            private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(default,
                default, NetworkVariableWritePermission.Owner);
            private NetworkVariable<Vector3> scale = new NetworkVariable<Vector3>(default,
                default, NetworkVariableWritePermission.Owner);

            private void Start()
            {
                refSphereCollider = GetComponent<SphereCollider>();
                ResetTransform();
                minScale = minScaleFactor * defaultScale;
                maxScale = maxScaleFactor * defaultScale;
            }

            private void OnEnable()
            {
                refRenderer = GetComponentInChildren<Renderer>();
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                mpb.SetVector(colorID, currentColor.Value);
                refRenderer.SetPropertyBlock(mpb);
            }

            public override void OnNetworkSpawn()
            {
                distanceAndScaleAction.action.performed += DistanceAndScale;

                currentColor.OnValueChanged += ApplyCurrentColorChange;
                position.OnValueChanged += ApplyPositionToTransform;
                scale.OnValueChanged += ApplyScaleToTransform;
            }

            public override void OnNetworkDespawn()
            {
                distanceAndScaleAction.action.performed -= DistanceAndScale;

                currentColor.OnValueChanged -= ApplyCurrentColorChange;
                position.OnValueChanged -= ApplyPositionToTransform;
                scale.OnValueChanged -= ApplyScaleToTransform;
            }

            private void ApplyCurrentColorChange(Color _previous, Color _current)
            {
                mpb.SetVector(colorID, _current);
                refRenderer.SetPropertyBlock(mpb);
            }

            private void ApplyPositionToTransform(Vector3 _past, Vector3 _current)
            {
                if (!IsOwner)
                {
                    transform.localPosition = position.Value;
                }
            }

            private void ApplyScaleToTransform(Vector3 _past, Vector3 _current)
            {
                if (!IsOwner)
                {
                    transform.localScale = scale.Value;
                }
            }

            /// <summary>
            /// Collision Event with objects in the scene.
            /// </summary>
            private void OnTriggerEnter(Collider collider)
            {
                refGrpMkgManager.CheckCollision(collider.gameObject);
            }

            /// <summary>
            /// Compares if vector <paramref name="_a"/> is less than
            /// <paramref name="_b"/> component-wise.
            /// </summary>
            private bool CompareVec3(Vector3 _a, Vector3 _b)
            {
                return (_a.x < _b.x && _a.y < _b.y && _a.z < _b.z);
            }

            /// <summary>
            /// Manages the data from the Joystick and processes it to
            /// move the gameobject and scale it.
            /// </summary>
            private void DistanceAndScale(InputAction.CallbackContext _ctx)
            {
                Vector2 _das = _ctx.ReadValue<Vector2>();
                if (!IsInDeadZone(_das.y))
                {
                    ManageDistance(_das.y);
                }
                if (!IsInDeadZone(_das.x))
                {
                    ManageScale(_das.x);
                }
            }

            /// <summary>
            /// A hard coded method returning true if Abs(<paramref name="_value"/>)
            /// lower than <paramref name="_threshold"/>. Returns false otherwise.
            /// </summary>
            /// <remarks>Used to clamp Joystick input data since I couldn't
            /// make the built-in deadzone work as intended.</remarks>
            private bool IsInDeadZone(float _value, float _threshold = 0.5f)
            {
                return Mathf.Abs(_value) < _threshold;
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
                isActive.Value = _active;
                if (_active)
                {
                    SetHighlightServerRpc();
                }
                else
                {
                    UnsetHighlightServerRpc();
                }
            }

            /// <summary>
            /// Moves the selector forward or backward.
            /// </summary>
            /// <param name="_mvtFactor">If greater than 0 then forward movement;
            /// If lower than 0 then backward movement.</param>
            private void ManageDistance(float _mvtFactor)
            {
                if (IsOwner)
                {
                    Vector3 target = transform.localPosition + _mvtFactor * movementSpeed * Vector3.forward;
                    float _d = (target - defaultPosition).z;
                    if (_d < 0)
                    {
                        target = defaultPosition;
                    }
                    if (_d > maxDistance)
                    {
                        target = transform.localPosition;
                    }
                    transform.localPosition = Vector3.SmoothDamp(
                                                transform.localPosition,
                                                target,
                                                ref mvtVelocity,
                                                0.1f);
                    position.Value = transform.localPosition;
                }
            }

            /// <summary>
            /// Scales the selector up or down.
            /// </summary>
            /// <param name="_mvtFactor">If greater than 0 then scales up;
            /// If lower than 0 then scales down.</param>
            private void ManageScale(float _growthFactor)
            {
                if (IsOwner)
                {

                    Vector3 target = transform.localScale + _growthFactor * growthSpeed * Vector3.one;

                    if (CompareVec3(target, minScale))
                    {
                        target = minScaleFactor * defaultScale;
                    }
                    if (CompareVec3(maxScale, target))
                    {
                        target = maxScaleFactor * defaultScale;
                    }

                    transform.localScale = Vector3.SmoothDamp(
                                                transform.localScale,
                                                target,
                                                ref growthVelocity,
                                                0.1f);
                    scale.Value = transform.localScale;
                }
            }

            /// <summary>
            /// Resets the gameobject trasnforms to start position and scale.
            /// </summary>
            public void ResetTransform()
            {
                transform.localPosition = defaultPosition;
                transform.localScale = defaultScale;
            }

            #region - IHighlightable -

            /// <inheritdoc/>
            [ServerRpc(RequireOwnership = false)]
            public void SetDefaultServerRpc()
            {
                m_currentColor.Value = m_defaultColor;
            }

            /// <inheritdoc/>
            public void SetDefaultColor(Color _c)
            {
                m_defaultColor = _c;
            }

            /// <inheritdoc/>
            [ServerRpc(RequireOwnership = false)]
            public virtual void SetHighlightServerRpc()
            {
                m_currentColor.Value = m_highlightColor;
            }

            /// <inheritdoc/>
            public void SetHighlightColor(Color _c)
            {
                m_highlightColor = _c;
            }

            /// <inheritdoc/>
            [ServerRpc(RequireOwnership = false)]
            public virtual void UnsetHighlightServerRpc()
            {
                if (!forceHighlight)
                {
                    m_currentColor.Value = m_defaultColor;
                }
            }
            #endregion
        }
    }
}

