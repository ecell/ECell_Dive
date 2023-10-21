using UnityEngine;
using UnityEngine.UI;
using ECellDive.Interfaces;

namespace ECellDive.UI
{
	/// <summary>
	/// The class to manage the main menu UI.
	/// </summary>
	public class MainMenuManager : MonoBehaviour,
									IInteractibility
	{
        #region - IInteractibility Members -
        /// <summary>
        /// The field for the <see cref="previousInteractibility"/> property.
        /// </summary>
        private bool[] m_previousInteractibility;

        ///<inheritdoc/>
        public bool[] previousInteractibility
        {
            get => m_previousInteractibility;
        }

        /// <summary>
        /// The field for the <see cref="targetGroup"/> property.
        /// </summary>
        [SerializeField] private Selectable[] m_targetGroup;

        ///<inheritdoc/>
        public Selectable[] targetGroup
        {
            get => m_targetGroup;
        }
        #endregion

        #region - IInteractibility Methods -
        /// <inheritdoc/>
        public void ForceGroupInteractibility(bool _interactibility)
        {
            for (int i = 0; i < m_targetGroup.Length; i++)
            {
                m_previousInteractibility[i] = m_targetGroup[i].interactable;
                m_targetGroup[i].interactable = _interactibility;
            }
        }

        /// <inheritdoc/>
        public void RestoreGroupInteractibility()
        {
            bool interactibility = true;
            for (int i = 0; i < m_targetGroup.Length; i++)
            {
                interactibility = m_targetGroup[i].interactable;
                m_targetGroup[i].interactable = m_previousInteractibility[i];
                m_previousInteractibility[i] = interactibility;
            }
        }

        /// <inheritdoc/>
        public void RestoreSingleInteractibility(int targetIdx)
        {
            bool interactibility = m_targetGroup[targetIdx].interactable;
            m_targetGroup[targetIdx].interactable = m_previousInteractibility[targetIdx];
            m_previousInteractibility[targetIdx] = interactibility;
        }

        /// <inheritdoc/>
        public void StoreGroupInteractibility()
        {
            for (int i = 0; i < m_targetGroup.Length; i++)
            {
                m_previousInteractibility[i] = m_targetGroup[i].interactable;
            }
        }

        /// <inheritdoc/>
        public void StoreSingleInteractibility(int targetIdx)
        {
            m_previousInteractibility[targetIdx] = m_targetGroup[targetIdx].interactable;
        }

        /// <inheritdoc/>
        public void SwitchGroupInteractibility()
        {
            for (int i = 0; i < m_targetGroup.Length; i++)
            {
                m_previousInteractibility[i] = m_targetGroup[i].interactable;
                m_targetGroup[i].interactable = !m_targetGroup[i].interactable;
            }
        }

        /// <inheritdoc/>
        public void SwitchSingleInteractibility(int _targetIdx)
        {
            m_previousInteractibility[_targetIdx] = m_targetGroup[_targetIdx].interactable;
            m_targetGroup[_targetIdx].interactable = !m_targetGroup[_targetIdx].interactable;
        }
        #endregion
    }

}
