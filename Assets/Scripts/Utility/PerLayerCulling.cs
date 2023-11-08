using UnityEngine;

namespace ECellDive.Utility
{
	/// <summary>
	/// A utility class to set the culling distance of each layer.
	/// This is a performance optimization.
	/// </summary>
	public class PerLayerCulling : MonoBehaviour
	{
		/// <summary>
		/// The culling distance for each layer.
		/// </summary>
		public float[] layerCulling = new float[32];
		void Start()
		{
			Camera camera = GetComponent<Camera>();
			camera.layerCullDistances = layerCulling;
		}
	}
}

