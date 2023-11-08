using System;
using System.Collections;
using UnityEngine;

namespace ECellDive.Utility
{
	/// <summary>
	/// Utility class to set the start and end positions of a LineRenderer.
	/// It is particularly usefull to update the connections between an
	/// info display and its module, node, edge, etc...
	/// </summary>
	/// <remarks>
	/// Should be simplified. There is legacy code in there.
	/// </remarks>
	/// <todo>Should be simplified. There is legacy code in there.</todo>
	public class LinePositionHandler : MonoBehaviour
	{
		/// <summary>
		/// An enum to specify what referential to use for the start and end positions.
		/// </summary>
		[Serializable]
		public enum PositionScope { local, world, delta, _override, startLocal}

		/// <summary>
		/// The referential to use for the start position
		/// </summary>
		public PositionScope startPositionScope;
		
		/// <summary>
		/// The transform of the start position.
		/// </summary>
		public Transform start;

		/// <summary>
		/// An arbitrary position to use for the start position.
		/// Only used if <see cref="startPositionScope"/> is set to <see cref="PositionScope._override"/>.
		/// </summary>
		public Vector3 overrideStart;

		/// <summary>
		/// The referential to use for the end position
		/// </summary>
		public PositionScope endPositionScope;

		/// <summary>
		/// The transform of the end position.
		/// </summary>
		public Transform end;

		/// <summary>
		/// An arbitrary position to use for the end position.
		/// Only used if <see cref="endPositionScope"/> is set to <see cref="PositionScope._override"/>.
		/// </summary>
		public Vector3 overrideEnd;

		/// <summary>
		/// The reference to the LineRenderer to update.
		/// </summary>
		public LineRenderer refLineRenderer;

		/// <summary>
		/// That is a hack to make sure the line is updated after the objects are spawned
		/// and may have moved. There is probably a better way to do this.
		/// </summary>
		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			RefreshLinePositions();
		}

		/// <summary>
		/// Updates the start position of the LineRenderer.
		/// <list type="bullet">
		/// <item>
		/// If <see cref="startPositionScope"/> is set to <see cref="PositionScope.local"/>,
		/// <code>start.localPosition</code> is used.
		/// </item>
		/// 
		/// <item>	
		/// If <see cref="startPositionScope"/> is set to <see cref="PositionScope.world"/>,
		/// <code>start.position</code> is used.
		/// </item>
		/// 
		/// <item>
		/// If <see cref="startPositionScope"/> is set to <see cref="PositionScope.delta"/>,
		/// <code>start.position - end.position</code> is used.
		/// </item>
		/// 
		/// <item>
		/// If <see cref="startPositionScope"/> is set to <see cref="PositionScope._override"/>,
		/// <code>overrideStart</code> is used.
		/// </item>
		/// 
		/// </list>
		/// </summary>
		public void RefreshLineStartPosition()
		{
			switch (startPositionScope)
			{
				case (PositionScope.local):
					refLineRenderer.SetPosition(0, start.localPosition);
					break;
				case (PositionScope.world):
					refLineRenderer.SetPosition(0, start.position);
					break;
				case (PositionScope.delta):
					refLineRenderer.SetPosition(0, start.position - end.position);
					break;
				case (PositionScope._override):
					refLineRenderer.SetPosition(0, overrideStart);
					break;
			}
				
		}

		/// <summary>
		/// Updates the end position of the LineRenderer.
		/// <list type="bullet">
		/// <item>
		/// If <see cref="endPositionScope"/> is set to <see cref="PositionScope.local"/>,
		/// <code>end.localPosition</code> is used.
		/// </item>
		/// 
		/// <item>	
		/// If <see cref="endPositionScope"/> is set to <see cref="PositionScope.world"/>,
		/// <code>end.position</code> is used.
		/// </item>
		/// 
		/// <item>
		/// If <see cref="endPositionScope"/> is set to <see cref="PositionScope.delta"/>,
		/// <code>end.position - start.position</code> is used.
		/// </item>
		/// 
		/// <item>
		/// If <see cref="endPositionScope"/> is set to <see cref="PositionScope._override"/>,
		/// <code>overrideStart</code> is used.
		/// </item>
		/// 
		/// <item>
		/// If <see cref="endPositionScope"/> is set to <see cref="PositionScope.startLocal"/>,
		/// <code>start.InverseTransformPoint(end.position)</code> is used.
		/// </item>
		/// 
		/// </list>
		/// </summary>
		public void RefreshLineEndPosition()
		{
			switch (endPositionScope)
			{
				case (PositionScope.local):
					refLineRenderer.SetPosition(1, end.localPosition);
					break;
				case (PositionScope.world):
					refLineRenderer.SetPosition(1, end.position);
					break;
				case (PositionScope.delta):
					refLineRenderer.SetPosition(1, end.position - start.position);
					break;
				case (PositionScope._override):
					refLineRenderer.SetPosition(1, overrideEnd);
					break;
				case (PositionScope.startLocal):
					refLineRenderer.SetPosition(1, start.InverseTransformPoint(end.position));
					break;
			}
		}

		/// <summary>
		/// Performs both RefreshLineStartPosition() and 
		/// RefreshLineEndPosition().
		/// </summary>
		public void RefreshLinePositions()
		{
			RefreshLineStartPosition();
			RefreshLineEndPosition();
		}
	}
}
