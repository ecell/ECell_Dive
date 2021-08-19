using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Checks for Axis alignements. Partly copied from the assets in
/// Unity Learn VR Beginner Escape room.
/// </summary>
public class OnRotationConstraint : MonoBehaviour
{
    [System.Serializable]
    public class AxisMatch
    {
        public Vector3 LocalAxis;
        public Vector3 TargetAxis;
        [Range(0.0f, 1.0f)] public float Tolerance = 0.3f;
    }

    public Transform refCameraTransform;

    public AxisMatch[] RequiredMatch;

    [Tooltip("Actions to check")]
    public InputActionReference refAction = null;

    // When the button is pressed
    public UnityEvent OnConstraintValidated = new UnityEvent();

    // When the button is released
    public UnityEvent OnConstraintViolated = new UnityEvent();

    private bool constraintsMet = false;

    private void Awake()
    {
        refAction.action.performed += DoConstraintCheck;
    }

    private void OnDestroy()
    {
        refAction.action.performed -= DoConstraintCheck;
    }

    private void OnEnable()
    {
        refAction.action.Enable();
    }

    private void OnDisable()
    {
        refAction.action.Disable();
    }


    private bool ConstraintCheck()
    {
        bool _allCheck = true;

        for (int i = 0; i < RequiredMatch.Length && _allCheck; ++i)
        {
            _allCheck &= AxisCheck(RequiredMatch[i].LocalAxis, RequiredMatch[i].TargetAxis, RequiredMatch[i].Tolerance);
        }

        return _allCheck;
    }

    private bool AxisCheck(Vector3 _localAxis, Vector3 _targetAxis, float _tolerance)
    {
        Vector3 worldLocal = transform.TransformVector(_localAxis);
        Vector3 worldTarget = refCameraTransform.TransformVector(_targetAxis);

        float dot = Vector3.Dot(worldLocal, worldTarget);

        return (dot > 1.0f - _tolerance);
    }

    private void ConstraintValidated()
    {
        OnConstraintValidated.Invoke();
    }

    private void ConstraintViolated()
    {
        OnConstraintViolated.Invoke();
    }

    private void DoConstraintCheck(InputAction.CallbackContext context)
    {
        if (ConstraintCheck())
        {
            if (!constraintsMet)
            {
                ConstraintValidated();
                constraintsMet = true;
            }
        }
        else
        {
            if (constraintsMet)
            {
                ConstraintViolated();
                constraintsMet = false;
            }
        }
    }
}
