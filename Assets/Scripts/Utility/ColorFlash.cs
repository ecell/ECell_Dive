using System.Collections;
using UnityEngine;

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
        private MaterialPropertyBlock[] mpbs;
        private int colorID;
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
            colorID = Shader.PropertyToID("_Color");
            for (int i = 0; i < renderers.Length; i++)
            {
                mpbs[i] = new MaterialPropertyBlock();
                renderers[i].GetPropertyBlock(mpbs[i]);
                startColors[i] = mpbs[i].GetVector(colorID);
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
                startColors[i] = mpbs[i].GetVector(colorID);
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

                    mpbs[i].SetVector(colorID, processColor);
                    renderers[i].SetPropertyBlock(mpbs[i]);
                }
                currentTime += Time.deltaTime;
                yield return null;
            }
            isFlashing = false;
        }
    }
}

