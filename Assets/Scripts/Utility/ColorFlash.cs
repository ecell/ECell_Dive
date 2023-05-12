using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ECellDive.Utility
{
    public class ColorFlash : MonoBehaviour
    {
        public Color[] flashColors;
        [Min(0.001f)] public float period = 0.33f;
        [Min(1)] public int nbPeriods = 3;

        private float currentTime = 0f;
        private float endTime = 0f;
        private float omega; //angular frequency

        [SerializeField] private Renderer[] renderers;
        [SerializeField] private string[] propertyNames;
        private MaterialPropertyBlock[] mpbs;
        private int[] colorIDs;
        private Color[] startColors;

        private Color flashColor;
        private Color processColor;

        private bool isFlashing = false;

        void Awake()
        {
            omega = 2*Mathf.PI / period;
            endTime = nbPeriods * period;

            startColors = new Color[renderers.Length];
            mpbs = new MaterialPropertyBlock[renderers.Length];
            colorIDs = new int[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                colorIDs[i] = Shader.PropertyToID(propertyNames[i]);
                mpbs[i] = new MaterialPropertyBlock();
                renderers[i].GetPropertyBlock(mpbs[i]);
                startColors[i] = mpbs[i].GetVector(colorIDs[i]);
            }
        }

        public void Flash(int _flashColorIndex)
        {
            //Stops the previous coroutine
            if (isFlashing)
            {
                StopCoroutine(FlashC());
            }

            flashColor = flashColors[_flashColorIndex];
            currentTime = 0f;
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].GetPropertyBlock(mpbs[i]);
                startColors[i] = mpbs[i].GetVector(colorIDs[i]);
            }

            StartCoroutine(FlashC());
        }

        private IEnumerator FlashC()
        {
            isFlashing = true;
            while(currentTime < endTime)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    processColor = Color.Lerp(startColors[i], flashColor, 0.5f * Mathf.Sin(omega * currentTime - 0.5f * Mathf.PI) + 0.5f);

                    mpbs[i].SetVector(colorIDs[i], processColor);
                    renderers[i].SetPropertyBlock(mpbs[i]);
                }
                currentTime += Time.deltaTime;
                yield return null;
            }
            isFlashing = false;
        }
    }
}

