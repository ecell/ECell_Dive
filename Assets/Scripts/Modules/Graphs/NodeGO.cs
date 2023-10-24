using UnityEngine;

using ECellDive.Interfaces;
using ECellDive.Utility.Data.Graph;
using UnityEngine.Rendering;

namespace ECellDive.Modules
{
	/// <summary>
	/// The class to manage a node defined by <see cref="ECellDive.Utility.Data.Graph.Node"/>.
	/// and nothing more.
	/// </summary>
	public class NodeGO : Module, INodeGO<Node>
	{
		#region - INodeGO Members -
		/// <summary>
		/// The field for the property <see cref="nodeData"/>.
		/// </summary>
		[Header("Node Parameters")]
		[SerializeField] private Node m_nodeData;

		/// <inheritdoc/>
		public Node nodeData
		{
			get => m_nodeData;
			private set => m_nodeData = value;
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
		#endregion

		public void SetPosition(Vector3 _position, float _positionScaleFactor)
		{
			transform.localPosition = _position * _positionScaleFactor;
		}

		public void SetNamePosition(float _sizeScaleFactor)
		{
			nameTextFieldContainer.transform.localPosition += Vector3.up * 2f * _sizeScaleFactor;
		}

		public void SetScale(Vector3 _scale, float _sizeScaleFactor)
		{
			transform.localScale = _scale * _sizeScaleFactor;

			//Compensating the scale factor so that the UI remains the same size (i.e. readable)
			for (int i = 0; i < refInfoTagsContainer.transform.childCount; i++)
			{
				refInfoTagsContainer.transform.GetChild(i).localScale /= _sizeScaleFactor;
			}
			nameTextFieldContainer.transform.localScale /= _sizeScaleFactor;
		}

		#region - INodeGO<Node> Methods -
		public void SetNodeData(Node _nodeData)
		{
			nodeData = _nodeData;

			informationString = $"ID: {nodeData.ID}\n" +
								$"Name: {nodeData.name}";
		}
		#endregion
	}
}
