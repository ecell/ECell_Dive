using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using ECellDive.Utility;

namespace ECellDive
{
    namespace Modules
    {
        public class Module : MonoBehaviour
        {
            public GameObject refCamera;
            public TextMeshProUGUI refName;
            public InputActionReference refDiveAction;

            public LayerMask refInteractorTargetLayer;
            public XRBaseInteractable refInteractable;
            public bool isFocused = false;

            private void Awake()
            {
                refDiveAction.action.performed += e => DiveIn();
            }

            protected virtual void DiveIn()
            {
                if (isFocused)
                {
                    Debug.Log("Base Dive In");
                    //scene transition animation launch
                }
            }

            private void OnEnable()
            {
                refDiveAction.action.Enable();
            }

            private void OnDisable()
            {
                refDiveAction.action.Disable();
            }

            public void CheckFocusOrigin()
            {
                if (refInteractable != null)
                {
                    foreach (XRBaseInteractor _interactor in refInteractable.hoveringInteractors)
                    {
                        if (_interactor.interactionLayerMask == refInteractorTargetLayer)
                        {
                            isFocused = true;
                        }
                    }
                    //Debug.Log(isFocused);
                }
            }

            public void Unfocus()
            {
                isFocused = false;
            }

            public void SetName(string _name)
            {
                refName.text = _name;
            }

            public void ShowNameToPlayer()
            {
                Positioning.UIFaceTarget(refName.gameObject.transform.parent.gameObject, refCamera.transform);
            }
        }
    }
}
