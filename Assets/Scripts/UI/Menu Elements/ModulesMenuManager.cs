using UnityEngine;
using UnityEngine.UI;
using ECellDive.Interfaces;

namespace ECellDive.UI
{
	/// <summary>
	/// The class to manage the module menu UI.
	/// </summary>
	public class ModulesMenuManager : MonoBehaviour,
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

		private void Start()
		{
			m_previousInteractibility = new bool[targetGroup.Length];
			ForceGroupInteractibility(false);
			StoreGroupInteractibility();
		}

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
        public void ForceSingleInteractibility(int targetIdx, bool _interactibility)
        {
            m_previousInteractibility[targetIdx] = m_targetGroup[targetIdx].interactable;
            m_targetGroup[targetIdx].interactable = _interactibility;
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
		public void RestoreSingleInteractibility(int _targetIdx)
		{
			bool interactibility = m_targetGroup[_targetIdx].interactable;
			m_targetGroup[_targetIdx].interactable = m_previousInteractibility[_targetIdx];
			m_previousInteractibility[_targetIdx] = interactibility;
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
		public void StoreSingleInteractibility(int _targetIdx)
		{
			m_previousInteractibility[_targetIdx] = m_targetGroup[_targetIdx].interactable;
		}

		/// <inheritdoc/>
		public void SwitchGroupInteractibility()
		{
			for(int i = 0; i < m_targetGroup.Length; i++)
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