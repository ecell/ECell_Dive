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
#if UNITY_EDITOR
        /// <summary>
        /// Set to true if you want to skip this step.
        /// ONLY IN EDITOR.
        /// </summary>
        /// <remarks>
        /// Usefull when designing the tutorial.
        /// </remarks>
        public bool skip = false;
#endif

        [Header("Global Step Members")]
        #region - ITutorialStep Members-
        [SerializeField] private string m_goal;
        public string goal { get => m_goal; }

        [SerializeField] private string m_task;

        public string task { get => m_task; }

        [SerializeField, TextArea] private string m_details;
        public string details { get => m_details; }

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

