using UnityEngine;

using ECellDive.Interfaces;
using ECellDive.Utility.Data.Graph;

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

		/// <summary>
		/// Sets the local position (i.e. relative to its parent) of the node.
		/// </summary>
		/// <param name="_position">
		/// The base position of the node.
		/// </param>
		/// <param name="_positionScaleFactor">
		/// A scalar to scale the position of the node.
		/// </param>
		public void SetPosition(Vector3 _position, float _positionScaleFactor)
		{
			transform.localPosition = _position * _positionScaleFactor;
		}

		/// <summary>
		/// Sets the position of the <see cref="ECellDive.Modules.Module.nameTextFieldContainer"/>
		/// to compensate for the scaling of the node. The position is scaled by the
		/// <paramref name="_sizeScaleFactor"/> which should be the same as the
		/// one used in <see cref="SetScale(Vector3, float)"/>.
		/// </summary>
		/// <param name="_sizeScaleFactor">
		/// The scalar initially used to scale the size of the node.
		/// </param>
		public void SetNamePosition(float _sizeScaleFactor)
		{
			nameTextFieldContainer.transform.localPosition += Vector3.up * 3f * _sizeScaleFactor;
		}

		/// <summary>
		/// Scale the node by the <paramref name="_sizeScaleFactor"/>. The scale is applied
		/// to the <see cref="ECellDive.Modules.Module.transform"/> of the node. The UI
		/// elements are scaled back to their original size.
		/// </summary>
		/// <param name="_scale">
		/// The base scale of the node.
		/// </param>
		/// <param name="_sizeScaleFactor">
		/// A scalar to scale the size of the node.
		/// </param>
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
		/// <inheritdoc/>
		public void SetNodeData(Node _nodeData)
		{
			nodeData = _nodeData;

			informationString = $"ID: {nodeData.ID}\n" +
								$"Name: {nodeData.name}";
		}
		#endregion
	}
}
