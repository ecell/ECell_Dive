using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.Utility.Data;
using ECellDive.Utility.PlayerComponents;

namespace ECellDive.Tutorials
{
	/// <summary>
	/// The step 8 of the tutorial on controls.
	/// Learn how to make a group of objects with the ray selector.
	/// </summary>
	public class ControlsStep8 : Step
	{
		[Header("Local Step Members")]
		public LeftRightData<InputActionReference> groupSelect;
		public GameObject[] targetGroupContent;

		private bool validGroup;

		public override bool CheckCondition()
		{
			return validGroup;
		}

		private void CheckGroupComposition(InputAction.CallbackContext _ctx)
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

		public override void Conclude()
		{
			base.Conclude();

			groupSelect.left.action.performed -= CheckGroupComposition;
			groupSelect.right.action.performed -= CheckGroupComposition;
		}

		public override void Initialize()
		{
			base.Initialize();
			
			groupSelect.left.action.performed += CheckGroupComposition;
			groupSelect.right.action.performed += CheckGroupComposition;
		}
	}
}

