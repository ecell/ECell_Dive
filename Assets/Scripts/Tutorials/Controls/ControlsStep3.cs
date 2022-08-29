using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Interfaces;
using ECellDive.Utility;


namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 3 of the tutorial on controls.
    /// Learn how to grab and move objects.
    /// </summary>
    public class ControlsStep3 : Step
    {
        [Header("Local Step Members")]
        public GameObject module;
        public GameObject targetArea;
        public ushort targetCollision = 5;
        private ushort collisionsDetected = 0;

        private void CheckCollision(Collider _other)
        {
            if (_other.gameObject == module)
            {
                collisionsDetected++;
                RelocateTarget();
            }
        }

        public override bool CheckCondition()
        {
            return collisionsDetected >= targetCollision;
        }

        public override void Initialize()
        {
            base.Initialize();

            targetArea.GetComponent<TriggerBroadcaster>().onTriggerEnter += CheckCollision;

            //Enable the InfoTags of the grip trigger.
            StaticReferencer.Instance.refInfoTags[5].SetActive(true);
            StaticReferencer.Instance.refInfoTags[10].SetActive(true);

            //Enable the InfoTags of the JoySticks.
            StaticReferencer.Instance.refInfoTags[3].SetActive(true);
            StaticReferencer.Instance.refInfoTags[8].SetActive(true);
        }

        private void RelocateTarget()
        {
            targetArea.transform.localPosition = new Vector3(Random.Range(-2f, 2f),
                                                             Random.Range(0f, 2f),
                                                             Random.Range(0f, 2f));
        }
    }
}

