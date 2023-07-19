using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;
using ECellDive.Utility;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 9 of the tutorial on controls.
    /// Learn how to make a group of objects with the volumetric selector.
    /// </summary>
    public class ControlsStep9 : Step
    {
        [Header("Local Step Members")]
        public LeftRightData<InputActionReference> switchGroupSelector;
        public GameObject[] targetGroupContent;

        private TriggerBroadcaster voluSelLeft;
        private TriggerBroadcaster voluSelRight;

        private bool validGroup;

        public override bool CheckCondition()
        {
            return validGroup;
        }

        private void CheckGroupComposition(Collider _collider)
        {
            if (targetGroupContent.Length == StaticReferencer.Instance.groupsMakingManager.groupMembers.Count)
            {
                bool _validGroup = true;
                foreach (GameObject _member in targetGroupContent)
                {
                    _validGroup &= StaticReferencer.Instance.groupsMakingManager.groupMembers.Contains(_member);
                }
                validGroup = _validGroup;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            //Destroying the group started at the previous step.
            StaticReferencer.Instance.groupsMakingManager.CancelGroup();

            voluSelLeft = StaticReferencer.Instance.volumetricSelectorManagers.left.GetComponent<TriggerBroadcaster>();
            voluSelRight = StaticReferencer.Instance.volumetricSelectorManagers.right.GetComponent<TriggerBroadcaster>();

            voluSelLeft.onTriggerEnter += CheckGroupComposition;
            voluSelRight.onTriggerEnter += CheckGroupComposition;
        }

        public override void Conclude()
        {
            base.Conclude();

            voluSelLeft.onTriggerEnter -= CheckGroupComposition;
            voluSelRight.onTriggerEnter -= CheckGroupComposition;
        }
    }
}

