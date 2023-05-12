using System.Collections;
using UnityEngine;

namespace ECellDive.Utility
{
    public class AnimationLoopWrapper : MonoBehaviour
    {
        public Animation animation;

        private bool playLoop = false;

        public void PlayLoop(string _anim)
        {
            playLoop = true;
            StartCoroutine(PlayLoopC(_anim));
        }

        private IEnumerator PlayLoopC(string _anim)
        {
            while (playLoop)
            {
                animation.Play(_anim);
                yield return new WaitWhile( () => animation.IsPlaying(_anim));
            }
        }

        public void StopLoop()
        {
            playLoop = false;
        }
    }
}
