using System.Collections.Generic;
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
        [SerializeField] private Selectable[] m_targetGroup;
        ///<inheritdoc/>
        public Selectable[] targetGroup
        {
            get => m_targetGroup;
        }



        #region - IInteractibility Methods -
        /// <inheritdoc/>
        public void ForceGroupInteractibility(bool _interactibility)
        {
            foreach (Selectable selectable in m_targetGroup)
            {
                selectable.interactable = _interactibility;
            }
        }

        /// <inheritdoc/>
        public void SwitchGroupInteractibility()
        {
            foreach (Selectable selectable in m_targetGroup)
            {
                selectable.interactable = !selectable.interactable;
            }
        }

        /// <inheritdoc/>
        public void SwitchSingleInteractibility(int _targetIdx)
        {
            m_targetGroup[_targetIdx].interactable = !m_targetGroup[_targetIdx].interactable;
        }
        #endregion
    }

}
