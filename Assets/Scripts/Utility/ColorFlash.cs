using System.Collections;
using UnityEngine;

namespace ECellDive.Utility
{
	/// <summary>
	/// A utility class to flash a color on an array of renderers.
	/// The flash is a sine wave defined by a period and a number of periods.
	/// The flash is done by lerping between the start colors of the renderers
	/// and the target colors.
	/// </summary>
	public class ColorFlash : MonoBehaviour
	{
		/// <summary>
		/// The colors to flash.
		/// It is a one-to-one mapping with the renderers (see <see cref="renderers"/>).
		/// </summary>
		public Color[] flashColors;

		/// <summary>
		/// The period of the sine wave.
		/// </summary>
		[Min(0.001f)] public float period = 0.33f;

		/// <summary>
		/// The number of periods of the sine wave.
		/// </summary>
		[Min(1)] public int nbPeriods = 3;

		/// <summary>
		/// The current time of the sine wave.
		/// </summary>
		private float currentTime = 0f;

		/// <summary>
		/// The end time of the sine wave.
		/// Corresponds to <see cref="nbPeriods"/> * <see cref="period"/>.
		/// </summary>
		private float endTime = 0f;

		/// <summary>
		/// The angular frequency of the sine wave.
		/// Corresponds to 2*PI / <see cref="period"/>.
		/// </summary>
		private float omega; //angular frequency

		/// <summary>
		/// The renderers to flash.
		/// </summary>
		[SerializeField] private Renderer[] renderers;

		/// <summary>
		/// The names of the color properties in the shaders of the renderers.
		/// </summary>
		[SerializeField] private string[] propertyNames;

		/// <summary>
		/// The material property blocks to flash the renderers
		/// without completely overriding their materials.
		/// </summary>
		private MaterialPropertyBlock[] mpbs;

		/// <summary>
		/// The IDs of the color properties in the shaders of the renderers.
		/// The are retrieved from <see cref="propertyNames"/>.
		/// </summary>
		private int[] colorIDs;

		/// <summary>
		/// An array to store the initial colors of the renderers so that
		/// they can be restored after the flash.
		/// </summary>
		private Color[] startColors;

		/// <summary>
		/// The current color of the flash.
		/// </summary>
		private Color flashColor;

		/// <summary>
		/// The color buffer to store the interpolated color of the flash.
		/// </summary>
		private Color processColor;

		/// <summary>
		/// A boolean to check if the flash is currently running.
		/// </summary>
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

		/// <summary>
		/// The public interface to flash a color.
		/// </summary>
		/// <param name="_flashColorIndex">
		/// The index of the color to flash in <see cref="flashColors"/>.
		/// </param>
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

		/// <summary>
		/// The coroutine to flash a color.
		/// This is where the interpolated color is computed thanks to
		/// the sine wave.
		/// </summary>
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

