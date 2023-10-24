using UnityEngine;

using ECellDive.Interfaces;
using ECellDive.Utility.Data.Graph;

namespace ECellDive.Modules
{
	/// <summary>
	/// The class to manage an edge defined by <see cref="ECellDive.Utility.Data.Graph.Edge"/>.
	/// and nothing more.
	/// </summary>
	public class EdgeGO : Module, IEdgeGO<Edge>
	{
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
			nameTextFieldContainer.transform.localPosition = 0.5f * (m_LineRenderers[0].GetPosition(0) + m_LineRenderers[0].GetPosition(1)) +
															_sizeScaleFactor * 1.5f * Vector3.up;
		}

		#region - IEdgeGO<Edge> Methods -
		/// <inheritdoc/>
		public void ReverseOrientation()
		{
			Vector3 startBuffer = m_LineRenderers[0].GetPosition(0);
			m_LineRenderers[0].SetPosition(0, m_LineRenderers[0].GetPosition(1));
			m_LineRenderers[0].SetPosition(1, startBuffer);
		}

		/// <inheritdoc/>
		public void SetCollider(Transform _start, Transform _end)
		{
			m_refBoxColliderHolder.transform.localPosition = 0.5f * (_start.localPosition + _end.localPosition);
			m_refBoxColliderHolder.transform.LookAt(_end);
			m_refBoxColliderHolder.transform.localScale = new Vector3(
															0.33f * Mathf.Max(m_LineRenderers[0].startWidth, m_LineRenderers[0].endWidth),//0.33f is custom for the inner size of the arrow texture
															0.33f * Mathf.Max(m_LineRenderers[0].startWidth, m_LineRenderers[0].endWidth),//0.33f is custom for the inner size of the arrow texture
															0.95f * Vector3.Distance(_start.localPosition, _end.localPosition));//0.95f is custom to avoid overlapping of the edge box collider with the nodes colliders
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
			m_LineRenderers[0].SetPosition(0, _start.localPosition);
			m_LineRenderers[0].SetPosition(1, _end.localPosition);
		}

		/// <inheritdoc/>
		public void SetLineRendererWidth()
		{
			m_LineRenderers[0].startWidth = defaultStartWidth;
			m_LineRenderers[0].endWidth = defaultEndWidth;
		}
		#endregion
	}
}