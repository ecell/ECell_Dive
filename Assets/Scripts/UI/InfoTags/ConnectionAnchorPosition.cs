using System.Collections;
using UnityEngine;

namespace ECellDive.UI
{
	/// <summary>
	/// Computes the position of an anchor in a 2D UI panel that is the 
	/// closest to its master object. A connection line will be drawn between
	/// the anchor and the master object.
	/// </summary>
	public class ConnectionAnchorPosition : MonoBehaviour
	{
		/// <summary>
		/// The reference to the transform of the object to which the anchor is attached.
		/// </summary>
		public Transform refMasterObject;

		/// <summary>
		/// The reference to the RectTransform of the UI attached to <see cref="refMasterObject"/>.
		/// by this connection.
		/// </summary>
		public RectTransform refAnchoredObject;

		/// <summary>
		/// The array of the corners of <see cref="refAnchoredObject"/>.
		/// </summary>
		private Vector3[] anchoredObjectCorners;

		/// <summary>
		/// The index of the corner of <see cref="refAnchoredObject"/> that is the
		/// closest to <see cref="refMasterObject"/>.
		/// </summary>
		private int closestCornerIndex = 0;

        private void Start()
        {
            SetClosestCorner();
        }

        /// <summary>
        /// Moves the anchor to the corner of refAnchoredObject that is
        /// the closest to refMasterObject.
        /// </summary>
        IEnumerator SetClosestCornerC()
		{
			yield return new WaitForEndOfFrame();

			anchoredObjectCorners = new Vector3[4];
			refAnchoredObject.GetWorldCorners(anchoredObjectCorners);

			float dist = Mathf.Infinity;
			for (var i = 0; i < 4; i++)
			{
				float testDist = Vector3.Distance(anchoredObjectCorners[i], refMasterObject.position);
				if ( testDist < dist)
				{
					dist = testDist;
					closestCornerIndex = i;
				}
			}
			transform.position = anchoredObjectCorners[closestCornerIndex];
		}

		/// <summary>
		/// The public method to fire the coroutine
		/// </summary>
		public void SetClosestCorner()
		{
			if (gameObject.activeInHierarchy)
			{
				StartCoroutine(SetClosestCornerC());
			}
		}
	}
}

