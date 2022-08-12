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
        public TMP_Text title;
        public TMP_Text goalContainer;
        public TMP_Text taskContainer;
        public TMP_Text detailsContainer;
        public List<Step> steps;
        public int officialNumberOfSteps;

        private string baseTitle;
        private int currentStep = 0;

        private void Start()
        {
            baseTitle = title.text;
            StartCoroutine(ImplementStep(currentStep));
        }

        public IEnumerator ImplementStep(int _stepIdx)
        {
#if UNITY_EDITOR
            if (steps[_stepIdx].skip)
            {
                steps[_stepIdx].Initialize();
                steps[_stepIdx].Conclude();
                NextStep();
                yield break;
            }
#endif

            steps[_stepIdx].Initialize();

            title.text = baseTitle + $" ({currentStep}/{officialNumberOfSteps})";
            goalContainer.text = $"Goal: " + steps[_stepIdx].goal;
            taskContainer.text = $"Task: " + steps[_stepIdx].task;
            detailsContainer.text = $"Details:\n" + steps[_stepIdx].details;

            yield return new WaitUntil(steps[_stepIdx].CheckCondition);

            steps[_stepIdx].Conclude();
        }

        public void NextStep()
        {
            Debug.Log($"Current step was number {currentStep}. " +
                "Checking if there are any left.");
            if (++currentStep < steps.Count)
            {
                Debug.Log($"Going for the next step numbered {currentStep}");
                StartCoroutine(ImplementStep(currentStep));
            }
        }
    }
}

