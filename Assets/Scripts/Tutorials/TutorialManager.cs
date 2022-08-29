using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;


namespace ECellDive.Tutorials
{
    
    public class TutorialManager : MonoBehaviour
    {
        [Header("General References")]
        public TMP_Text title;
        public TMP_Text goalContainer;
        public TMP_Text taskContainer;
        public TMP_Text detailsContainer;
        public TMP_Text globalMessageContainer;
        

        [Header("Chronology")]
        [TextArea] public string startMessage;

        public UnityEvent initializationInstructions;
        [Tooltip("Sometimes, you may define tutorial steps to set up" +
            "the tutorial's behavior. Therefore, those steps are " +
            "inivisible to the user and the number of step in the list " +
            "\"steps\" does not equal the number of steps the user will " +
            "read and execute. Thus, \"officialNumberOfSteps\" is here " +
            "to describe this actual total number (the one that will " +
            "be displayed on the title bar).")]
        public int officialNumberOfSteps;
        public List<Step> steps;

        [TextArea] public string endMessage;
        public UnityEvent conclusionInstructions;

        private string baseTitle;
        private int currentStep = 0;

        private void Start()
        {
            baseTitle = title.text;
            Initialize();
        }

        protected virtual void Conclude()
        {
            conclusionInstructions.Invoke();

            globalMessageContainer.text = endMessage;

            goalContainer.gameObject.SetActive(false);
            taskContainer.gameObject.SetActive(false);
            detailsContainer.gameObject.SetActive(false);
            globalMessageContainer.gameObject.SetActive(true);
        }

        protected virtual void Initialize()
        {
            initializationInstructions.Invoke();

            globalMessageContainer.text = startMessage;

            goalContainer.gameObject.SetActive(false);
            taskContainer.gameObject.SetActive(false);
            detailsContainer.gameObject.SetActive(false);
            globalMessageContainer.gameObject.SetActive(true);
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

            title.text = baseTitle + $" ({currentStep+1}/{officialNumberOfSteps})";
            goalContainer.text = $"Goal: " + steps[_stepIdx].goal;
            taskContainer.text = $"Task: " + steps[_stepIdx].task;
            detailsContainer.text = $"Details:\n" + steps[_stepIdx].details;

            yield return new WaitUntil(steps[_stepIdx].CheckCondition);

            steps[_stepIdx].Conclude();
        }

        public void NextStep()
        {
            if (++currentStep < steps.Count)
            {
                StartCoroutine(ImplementStep(currentStep));
            }
            else
            {
                Conclude();
            }
        }

        public virtual void Quit()
        {

        }

        public void StartTutorial()
        {
            goalContainer.gameObject.SetActive(true);
            taskContainer.gameObject.SetActive(true);
            detailsContainer.gameObject.SetActive(true);
            globalMessageContainer.gameObject.SetActive(false);

            StartCoroutine(ImplementStep(currentStep));
        }
    }
}
