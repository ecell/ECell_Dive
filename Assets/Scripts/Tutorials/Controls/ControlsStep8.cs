using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;
using ECellDive.Utility;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 8 of the tutorial on controls.
    /// Learn how to make a group of objects.
    /// </summary>
    public class ControlsStep8 : Step
    {
        [Header("Local Step Members")]
        public LeftRightData<InputActionReference> groupSelect;
        public GameObject[] targetGroupContent;

        private bool validGroup;

        public override bool CheckCondition()
        {
            return validGroup;
        }

        private void CheckGroupComposition(InputAction.CallbackContext _ctx)
        {
            Debug.Log("CheckGroupComposition");
            if (targetGroupContent.Length == StaticReferencer.Instance.groupsMakingManager.groupMembers.Count)
            {
                Debug.Log("Same size");
                bool _validGroup = true;
                foreach (GameObject _member in targetGroupContent)
                {
                    Debug.Log($"member {_member} is in group: {StaticReferencer.Instance.groupsMakingManager.groupMembers.Contains(_member)}");
                    _validGroup &= StaticReferencer.Instance.groupsMakingManager.groupMembers.Contains(_member);
                }
                validGroup = _validGroup;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            groupSelect.left.action.Enable();
            groupSelect.right.action.Enable();
            
            groupSelect.left.action.performed += CheckGroupComposition;
            groupSelect.right.action.performed += CheckGroupComposition;
        }
    }
}

