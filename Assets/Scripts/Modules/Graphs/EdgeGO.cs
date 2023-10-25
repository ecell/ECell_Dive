using System.Collections;
using UnityEngine;

using ECellDive.Interfaces;
using ECellDive.Utility.Data.Graph;
using ECellDive.Utility.Maths;

namespace ECellDive.Modules
{
	/// <summary>
	/// The class to manage an edge defined by <see cref="ECellDive.Utility.Data.Graph.Edge"/>.
	/// and nothing more.
	/// </summary>
	public class EdgeGO : Module, IEdgeGO<Edge>, IBezierCurve
	{
		#region - IBezierCurve Members -
		/// <summary>
		/// The field for the property <see cref="controlPoints"/>.
		/// </summary>
		private Vector3[] m_controlPoints;

		/// <inheritdoc/>
		public Vector3[] controlPoints
		{
			get => m_controlPoints;
			private set => m_controlPoints = value;
		}

		/// <summary>
		/// The field for the property <see cref="controlPointsCount"/>.
		/// </summary>
		private uint m_controlPointsCount;

		/// <inheritdoc/>
		public uint controlPointsCount
		{
			get => m_controlPointsCount;
			private set => m_controlPointsCount = value;
		}

		/// <summary>
		/// The field for the property <see cref="curvePoints"/>.
		/// </summary>
		private Vector3[] m_curvePoints;

		/// <inheritdoc/>
		public Vector3[] curvePoints
		{
			get => m_curvePoints;
			private set => m_curvePoints = value;
		}

		/// <summary>
		/// The field for the property <see cref="curvePointsCount"/>.
		/// </summary>
		[Header("IBezierCurve Parameters")]
		[SerializeField] private uint m_curvePointsCount;

		/// <inheritdoc/>
		public uint curvePointsCount
		{
			get => m_curvePointsCount;
			set => m_curvePointsCount = value;
		}
		#endregion

		#region - IEdgeGO<Edge> Members -
		/// <summary>
		/// The field for the property <see cref="edgeData"/>.
		/// </summary>
		private Edge m_edgeData;

		/// <inheritdoc/>
		public Edge edgeData
		{
			get => m_edgeData;
			private set => m_edgeData = value;
		}

		/// <summary>
		/// The field for the property <see cref="informationString"/>.
		/// </summary>
		private string m_informationString;

		/// <inheritdoc/>
		public string informationString
		{
			get => m_informationString;
			private set => m_informationString = value;
		}

		/// <summary>
		/// The field for the property <see cref="defaultStartWidth"/>.
		/// </summary>
		[Header("IEdgeGO<Edge> Parameters")]
		[SerializeField] private float m_defaultStartWidth;

		/// <inheritdoc/>
		public float defaultStartWidth
		{
			get => m_defaultStartWidth;
			private set => m_defaultStartWidth = value;
		}

		/// <summary>
		/// The field for the property <see cref="defaultEndWidth"/>.
		/// </summary>
		[SerializeField] private float m_defaultEndWidth;

		/// <inheritdoc/>
		public float defaultEndWidth
		{
			get => m_defaultEndWidth;
			private set => m_defaultEndWidth = value;
		}

		/// <summary>
		/// The field for the property <see cref="refLineRenderer"/>.
		/// </summary>
		[SerializeField] private GameObject m_refColliderHolder;

		/// <inheritdoc/>
		public GameObject refColliderHolder
		{
			get => m_refColliderHolder;
			set => m_refColliderHolder = value;
		}
		#endregion

		Mesh mesh;

		protected override void Awake()
		{
			base.Awake();
			mesh = new Mesh();
			SetControlPoints(new Vector3[4]);
		}

		/// <summary>
		/// Maps a gradient of colors on the edge.
		/// This is possible only if the edge uses the custom shader "Edge"
		/// (Assets/Resources/Shaders/Edge.shadergraph).
		/// </summary>
		/// <param name="'_start">
		/// The color at the start of the edge.
		/// </param>
		/// <param name="_end">
		/// The color at the end of the edge.
		/// </param>
		public void SetColorGradient(Color _start, Color _end)
		{
			mpb.SetFloat("_UseGradient", 1f);
			mpb.SetVector("_GradientColorA", _start);
			mpb.SetVector("_GradientColorB", _end);
			lineRenderers[0].SetPropertyBlock(mpb);
		}

		/// <summary>
		/// Sets the position of the <see cref="ECellDive.Modules.Module.nameTextFieldContainer"/>
		/// to compensate for the movement of the line renderer (<see
		/// cref="SetLineRendererPosition(Transform, Transform)"/>. The new position is
		/// in the middle of the line renderer and slightly above.
		/// </summary>
		/// <param name="_sizeScaleFactor">
		/// The scalar initially used to scale the size of the node.
		/// </param>
		public void SetNamePosition(float _sizeScaleFactor)
		{
			nameTextFieldContainer.transform.localPosition = 0.5f * (lineRenderers[0].GetPosition(0) + lineRenderers[0].GetPosition((int)curvePointsCount-1)) +
															_sizeScaleFactor * 1.5f * Vector3.up;
		}

		/// <summary>
		/// Delaying the by one frame to work around a bug in Unity 2020.3
		/// It will become obselete once we update the project to Unity 2021.X+.
		/// <see href="https://forum.unity.com/threads/why-do-instantiated-linerenderers-not-allow-bakemesh-in-unity.1139956/"/>
		/// </summary>
		private IEnumerator DelayedSetColliderC()
		{
			Debug.Log("Main Camera", Camera.main);
			yield return new WaitForEndOfFrame();
			
			refColliderHolder.transform.LookAt(Camera.main.transform.position);
			lineRenderers[0].BakeMesh(mesh);

			refColliderHolder.GetComponent<MeshCollider>().sharedMesh = mesh;
		}

		#region - IColorHighlightable Methods -

		public override void ApplyColor(Color _color)
		{
			mpb.SetFloat("_UseGradient", 0f);
			base.ApplyColor(_color);
		}

		#endregion

		#region - ICurve Methods -
		/// <inheritdoc/>
		public void AddControlPoint(Vector3 _point)
		{
			Vector3[] newControlPoints = new Vector3[controlPointsCount + 1];
			for (int i = 0; i < controlPointsCount; i++)
			{
				newControlPoints[i] = controlPoints[i];
			}
			newControlPoints[controlPointsCount] = _point;
			controlPoints = newControlPoints;
			controlPointsCount++;
		}

		/// <inheritdoc/>
		public Vector3[] Interpolate()
		{
			curvePoints = new Vector3[curvePointsCount];
			for (int i = 0; i < curvePointsCount; i++)
			{
				curvePoints[i] = Bezier.GetPoint(controlPoints, i / (curvePointsCount - 1f));
			}
			return curvePoints;
		}

		/// <inheritdoc/>
		public Vector3[] Interpolate(uint _curvePointsCount)
		{
			curvePoints = new Vector3[_curvePointsCount];
			for (int i = 0; i < _curvePointsCount; i++)
			{
				curvePoints[i] = Bezier.GetPoint(controlPoints, i / (_curvePointsCount - 1f));
			}
			curvePointsCount = _curvePointsCount;
			return curvePoints;
		}

		/// <inheritdoc/>
		public void SetControlPoint(int _index, Vector3 _point)
		{
			if (_index < 0 || _index >= controlPointsCount)
			{
				return;
			}
			controlPoints[_index] = _point;
		}

		/// <inheritdoc/>
		public void SetControlPoints(Vector3[] _points)
		{
			controlPoints = _points;
			controlPointsCount = (uint)_points.Length;
		}

		#endregion

		#region - IEdgeGO<Edge> Methods -
		/// <inheritdoc/>
		public void ReverseOrientation()
		{
			Vector3 startBuffer = lineRenderers[0].GetPosition(0);
			lineRenderers[0].SetPosition(0, lineRenderers[0].GetPosition(1));
			lineRenderers[0].SetPosition((int)curvePointsCount-1, startBuffer);
		}

		/// <inheritdoc/>
		public void SetCollider(Transform _start, Transform _end)
		{
			StartCoroutine(DelayedSetColliderC());
		}

		/// <inheritdoc/>
		public void SetDefaultWidth(float _start, float _end)
		{
			defaultStartWidth = _start;
			defaultEndWidth = _end;
		}

		/// <inheritdoc/>
		public void SetEdgeData(Edge _edgeData)
		{
			edgeData = _edgeData;

			informationString = $"ID: {edgeData.ID}\n" +
								$"Name: {edgeData.name}\n" +
								$"Source: {edgeData.source}\n" +
								$"Target: {edgeData.target}";
		}

		/// <inheritdoc/>
		public void SetLineRendererPosition(Transform _start, Transform _end)
		{
			//We move the edge to the middle of the two nodes.
			Vector3 start = transform.localPosition;
			transform.localPosition = 0.5f * (_start.localPosition + _end.localPosition);

			//We compute the offset between the new position and the old one.
			Vector3 offset = transform.localPosition - start;
			
			//We compensate the line renderer start and end points by the same offset.
			start = _start.localPosition - offset;
			Vector3 end = _end.localPosition - offset;

			//We create the control points a the 1/3 and 2/3 of the edge.
			//They are slightly offset from the edge by a vector perpendicular to the edge.
			//That vector is the normal to the plane defined by the edge and the vector (0,0,-1).
			Vector3 p1 = 0.33f * (end - start) + start;
			p1 += 0.33f * Vector3.Cross(end - start, Vector3.back).normalized;
			Vector3 p2 = 0.66f * (end - start) + start;
			p2 += 0.33f * Vector3.Cross(end - start, Vector3.back).normalized;

			//We assigned a 4 points array in the awake method.
			//We can update it here with the new control points.
			SetControlPoint(0, start);
			SetControlPoint(1, p1);
			SetControlPoint(2, p2);
			SetControlPoint(3, end);

			//We update the curve points array.
			Interpolate();

			//We update the line renderer.
			lineRenderers[0].positionCount = (int)curvePointsCount;
			lineRenderers[0].SetPositions(curvePoints);
		}

		/// <inheritdoc/>
		public void SetLineRendererWidth()
		{
			lineRenderers[0].startWidth = defaultStartWidth;
			lineRenderers[0].endWidth = defaultEndWidth;
		}
		#endregion
	}
}