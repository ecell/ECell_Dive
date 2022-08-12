using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ECellDive.Interfaces;

namespace ECellDive.Tutorials
{
    public class Step : MonoBehaviour,
                        ITutorialStep
    {
        [Header("Global Step Members")]
        #region - ITutorialStep Members-
        [SerializeField] private string m_goal;
        public string goal { get => m_goal; }

        [SerializeField] private string m_task;

        public string task { get => m_task; }

        [SerializeField, TextArea] private string m_details;
        public string details { get => m_details; }

        [SerializeField] private GameObject m_learningResource;
        public GameObject learningResource { get => m_learningResource; }

        [SerializeField] private UnityEvent m_initializationInstructions;
        public UnityEvent initializationInstructions { get => m_initializationInstructions; }

        [SerializeField] private UnityEvent m_conclusionInstructions;
        public UnityEvent conclusionInstructions { get => m_conclusionInstructions; }
        #endregion

        /// <inheritdoc/>
        public virtual bool CheckCondition()
        {
            return true;
        }

        /// <inheritdoc/>
        public virtual void Conclude()
        {
            Debug.Log("Conclude step " + gameObject.name);
            conclusionInstructions.Invoke();
        }

        /// <inheritdoc/>
        public virtual void Initialize()
        {
            Debug.Log("Initialize step " + gameObject.name);
            initializationInstructions.Invoke();
        }

    }
}

