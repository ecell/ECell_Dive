using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace UserActions
    {
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

            private void OnTriggerEnter(Collider collider)
            {
                refGrpMkgManager.CheckCollision(collider.gameObject);
            }

            public void DistanceAndScale(InputAction.CallbackContext _ctx)
            {
                Vector2 _das = _ctx.ReadValue<Vector2>();
                ManageDistance(MinClamping(_das.y));
                //ManageDistance(_das.y);
                ManageScale(MinClamping(_das.x));
                //ManageScale(_das.x);
            }

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

            private void ManageDistance(float _mvtFactor)
            {
                Vector3 target = transform.position + _mvtFactor * movementSpeed * transform.forward;
                transform.position = Vector3.SmoothDamp(
                                            transform.position,
                                            target,
                                            ref mvtVelocity,
                                            0.1f);
            }

            private void ManageScale(float _growthFactor)
            {
                Vector3 target = transform.localScale + _growthFactor * growthSpeed * Vector3.one;
                transform.localScale = Vector3.SmoothDamp(
                                                transform.localScale,
                                                target,
                                                ref growthVelocity,
                                                0.1f);
            }

            private float MinClamping(float _value)
            {
                if (_value < 0)
                {
                    if (_value > -0.5f)
                    {
                        return 0;
                    }
                    else
                    {
                        return _value;
                    }
                }
                else
                {
                    if (_value < 0.5f)
                    {
                        return 0;
                    }
                    else
                    {
                        return _value;
                    }
                }
            }

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

