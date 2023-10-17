using TMPro;
using UnityEngine;

namespace ECellDive.UI
{
	/// <summary>
	/// A utility class to trigger a surge and shrink animation on 
	/// the game object this script is attached to. Requires a
	/// reference to a text mesh to explain the surge and shrink to
	/// the player.
	/// </summary>
	/// <remarks>
	/// Initially created to animate information tags when switching
	/// input modes on a controller (see <see cref="ECellDive.Input.InputModeManager"/>).
	/// </remarks>
	public class SurgeAndShrinkInfoTag : MonoBehaviour
	{
		/// <summary>
		/// The reference to the animation component.
		/// </summary>
		public Animation anim;

		/// <summary>
		/// The reference to the 
		/// </summary>
		public TextMeshProUGUI refInfoTagText;

		/// <summary>
		/// Simply activates the game object.
		/// </summary>
		public void Activate()
		{
			gameObject.SetActive(true);
		}

		/// <summary>
		/// Simply deactivates the game object.
		/// </summary>
		public void Deactivate()
		{
			gameObject.SetActive(false);
		}

		/// <summary>
		/// Triggers the surge and shrink animation on the game object.
		/// Any previously playing animation is interrupted before
		/// restarting the animation.
		/// </summary>
		/// <param name="_text">
		/// The text to display in the text mesh explaining the surge.
		/// </param>
		public void SurgeAndShrink(string _text)
		{
			if (anim.isPlaying)
			{
				anim.Stop();
			}
			gameObject.SetActive(true);
			refInfoTagText.text = _text;
			anim.Play("SurgeAndShrink");
		}
	}
}

