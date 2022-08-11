using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using ECellDive.Interfaces;


namespace ECellDive.Tutorials
{
    
    public class TutorialManager : MonoBehaviour
    {
        public TMP_Text goalContainer;
        public TMP_Text taskContainer;
        public TMP_Text detailsContainer;
        public List<Step> steps;

        private int currentStep = 0;

        private void Start()
        {
            StartCoroutine(ImplementStep(currentStep));
        }

        public IEnumerator ImplementStep(int _stepIdx)
        {
            steps[_stepIdx].Initialize();

            goalContainer.text = "Goal: " + steps[_stepIdx].goal;
            taskContainer.text = "Task: " + steps[_stepIdx].task;
            detailsContainer.text = "Details:\n" + steps[_stepIdx].details;

            yield return new WaitUntil(steps[_stepIdx].CheckCondition);

            steps[_stepIdx].Conclude();

            Debug.Log("Tutorial finished.");
        }

        public void NextStep()
        {
            if (++currentStep < steps.Count)
            {
                ImplementStep(currentStep);
            }
        }
    }
}

