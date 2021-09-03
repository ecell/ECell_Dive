using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ECellDive
{
    namespace SceneManagement
    {
        public static class Loading
        {
            public static IEnumerator SwitchScene(int _sceneIndex, float _delay)
            {
                yield return new WaitForSeconds(_delay);

                AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneIndex);

                while (!operation.isDone)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }
}

