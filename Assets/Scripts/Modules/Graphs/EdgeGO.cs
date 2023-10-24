using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ECellDive.Interfaces;
using ECellDive.Utility.Data.Graph;
using static UnityEngine.GraphicsBuffer;

namespace ECellDive.Modules
{
	/// <summary>
	/// The class to manage an edge defined by <see cref="ECellDive.Utility.Data.Graph.Edge"/>.
	/// and nothing more.
	/// </summary>
	public class EdgeGO : Module, IEdgeGO<Edge>
	{
		/// <summary>
		/// The line renderer to draw the edge.
		/// </summary>
		[Header("Edge Parameters")]
		[SerializeField] private LineRenderer m_LineRenderer;

		#region - IEdgeGO<Edge> Members -
		/// <summary>
		/// The field for the property <see cref="edgeData"/>.
		/// </summary>
		[Header("IEdgeGO<Edge> Parameters")]
		[SerializeField] private Edge m_edgeData;

		/// <inheritdoc/>
		public Edge edgeData
		{
			get => m_edgeData;
			private set => m_edgeData = value;
		}

		/// <summary>
		/// The field for the property <see cref="informationString"/>.
		/// </summary>
		[SerializeField] private string m_informationString;

		/// <inheritdoc/>
		public string informationString
		{
			get => m_informationString;
			private set => m_informationString = value;
		}

		/// <summary>
		/// The field for the property <see cref="defaultStartWidth"/>.
		/// </summary>
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
		[SerializeField] private GameObject m_refBoxColliderHolder;

		/// <inheritdoc/>
		public GameObject refBoxColliderHolder
		{
			get => m_refBoxColliderHolder;
			set => m_refBoxColliderHolder = value;
		}
		#endregion

		public void SetNamePosition(float _sizeScaleFactor)
		{
			nameTextFieldContainer.transform.localPosition = 0.5f * (m_LineRenderer.GetPosition(0) + m_LineRenderer.GetPosition(1)) +
															_sizeScaleFactor * 1.5f * Vector3.up;
		}

		#region - IEdgeGO<Edge> Methods -
		public void ReverseOrientation()
		{
			Vector3 startBuffer = m_LineRenderer.GetPosition(0);
			m_LineRenderer.SetPosition(0, m_LineRenderer.GetPosition(1));
			m_LineRenderer.SetPosition(1, startBuffer);
		}

		public void SetCollider(Transform _start, Transform _end)
		{
			m_refBoxColliderHolder.transform.localPosition = 0.5f * (_start.localPosition + _end.localPosition);
			m_refBoxColliderHolder.transform.LookAt(_end);
			m_refBoxColliderHolder.transform.localScale = new Vector3(
															0.33f * Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),//0.33f is custom for the inner size of the arrow texture
															0.33f * Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),//0.33f is custom for the inner size of the arrow texture
															0.95f * Vector3.Distance(_start.localPosition, _end.localPosition));//0.95f is custom to avoid overlapping of the edge box collider with the nodes colliders
		}

		public void SetDefaultWidth(float _start, float _end)
		{
			defaultStartWidth = _start;
			defaultEndWidth = _end;
		}

		public void SetEdgeData(Edge _edgeData)
		{
			edgeData = _edgeData;

			informationString = $"ID: {edgeData.ID}\n" +
								$"Name: {edgeData.name}\n" +
								$"Source: {edgeData.source}\n" +
								$"Target: {edgeData.target}";
		}

		public void SetLineRendererPosition(Transform _start, Transform _end)
		{
			m_LineRenderer.SetPosition(0, _start.localPosition);
			m_LineRenderer.SetPosition(1, _end.localPosition);
		}

		public void SetLineRendererWidth()
		{
			m_LineRenderer.startWidth = defaultStartWidth;
			m_LineRenderer.endWidth = defaultEndWidth;
		}
		#endregion
	}
}