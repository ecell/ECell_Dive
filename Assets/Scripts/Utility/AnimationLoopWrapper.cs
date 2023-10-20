using System.Collections;
using UnityEngine;

namespace ECellDive.Utility
{
	/// <summary>
	/// A utility class to play an animation loop.
	/// </summary>
	[RequireComponent(typeof(Animation))]
	public class AnimationLoopWrapper : MonoBehaviour
	{
		/// <summary>
		/// The animation component.
		/// </summary>
		public Animation anim;

		/// <summary>
		/// The boolean to control the loop.
		/// </summary>
		private bool playLoop = false;

		/// <summary>
		/// The public interface to start the loop.
		/// </summary>
		/// <param name="_anim">
		/// The string name of the animation to play.
		/// </param>
		public void PlayLoop(string _anim)
		{
			playLoop = true;
			StartCoroutine(PlayLoopC(_anim));
		}

        /// <summary>
        /// The coroutine to play the loop.
        /// We keep calling the <code>anim.Play(_anim)</code> as soon as it finished
		/// until <see cref="playLoop"/> is false.
        /// </summary>
        /// <param name="_anim">
		/// The string name of the animation to play.
		/// </param>
        private IEnumerator PlayLoopC(string _anim)
		{
			while (playLoop)
			{
				anim.Play(_anim);
				yield return new WaitWhile( () => anim.IsPlaying(_anim));
			}
		}

		/// <summary>
		/// The public interface to stop the loop.
		/// </summary>
		public void StopLoop()
		{
			playLoop = false;
		}
	}
}
