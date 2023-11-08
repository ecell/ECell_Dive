using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive.Utility
{
	/// <summary>
	/// A simple component to always call <see cref="ILookAt.LookAt"/>
	/// on update.
	/// </summary>
	[RequireComponent(typeof(ILookAt))]
	public class AlwaysLookAt : MonoBehaviour
	{
		/// <summary>
		/// The reference to the <see cref="ILookAt"/> component used to 
		/// always call <see cref="ILookAt.LookAt"/> on update.
		/// It must be attached to the same game object as this component.
		/// </summary>
		private ILookAt refLookAtComponent;

		void Start()
		{
			refLookAtComponent = GetComponent<ILookAt>();
		}

		void Update()
		{
			refLookAtComponent.LookAt();
		}
	}
}

