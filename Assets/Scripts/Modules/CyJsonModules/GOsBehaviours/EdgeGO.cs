using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using ECellDive.UI;
using ECellDive.Interfaces;
using ECellDive.Utility.Data;

namespace ECellDive.Modules
{
	/// <summary>
	/// The class to manage the behaviour of an edge in the graph.
	/// </summary>
	/// <remarks>
	/// It is synchronized over the multiplayer network.
	/// </remarks>
	public class EdgeGO : GameNetModule,
							IEdgeGO, IModulateFlux
	{
		#region - IEdgeGO Members -
		/// <inheritdoc/>
		public IEdge edgeData { get; set; }
			
		/// <inheritdoc/>
		public string informationString { get; protected set; }
			
		/// <inheritdoc/>
		public float defaultStartWidth { get; protected set; }
			
		/// <inheritdoc/>
		public float defaultEndWidth { get; protected set; }

		/// <summary>
		/// The field for <see cref="refBoxColliderHolder"/>.
		/// </summary>
		[SerializeField] private GameObject m_refBoxColliderHolder;

		/// <inheritdoc/>
		public GameObject refBoxColliderHolder
		{
			get => m_refBoxColliderHolder;
			set => refBoxColliderHolder = m_refBoxColliderHolder;
		}
		#endregion

		#region - IModulateFlux Members -
		/// <summary>
		/// The field for <see cref="triggerKOActions"/>.
		/// </summary>
		[SerializeField] private LeftRightData<InputActionReference> m_triggerKOActions;

		/// <inheritdoc/>
		public LeftRightData<InputActionReference> triggerKOActions
		{
			get => m_triggerKOActions;
			set => m_triggerKOActions = value;
		}

		/// <summary>
		/// The field for <see cref="knockedOut"/>.
		/// </summary>
		private NetworkVariable<bool> m_knockedOut = new NetworkVariable<bool>(false);

		/// <inheritdoc/>
		public NetworkVariable<bool> knockedOut { get => m_knockedOut; protected set => m_knockedOut = value; }

		/// <summary>
		/// The field for <see cref="fluxLevel"/>.
		/// </summary>
		private NetworkVariable<float> m_fluxLevel = new NetworkVariable<float>();

		/// <inheritdoc/>
		public NetworkVariable<float> fluxLevel { get => m_fluxLevel; protected set => m_fluxLevel = value; }

		/// <summary>
		/// The field for <see cref="fluxLevelClamped"/>.
		/// </summary>
		private NetworkVariable<float> m_fluxLevelClamped = new NetworkVariable<float>();

		/// <inheritdoc/>
		public NetworkVariable<float> fluxLevelClamped { get => m_fluxLevelClamped; protected set => m_fluxLevelClamped = value; }
		#endregion

		/// <summary>
		/// A boolean to control whether the edge should be assigned its
		/// default color every time <see cref="ApplyCurrentColorChange(Color, Color)"/>
		/// is called.
		/// </summary>
		[Header("Customization Parameters")]
		public bool forceDefaultColor;

		/// <summary>
		/// A boolean to control whether the edge should be assigned its
		/// default width every time <see cref="SetLineRendererWidth"/> is called.
		/// </summary>
		public bool forceDefaultWidth;

		/// <summary>
		/// A value to control the scale of the starting edge's width.
		/// </summary>
		[Range(0, 1)] public float startWidthFactor = 1f;

		/// <summary>
		/// A value to control the scale of the ending edge's width.
		/// </summary>
		[Range(0, 1)] public float endWidthFactor = 1f;

		/// <summary>
		/// The ID of the property in the shader to control the visual
		/// of the edge (Knockedout or not).
		/// </summary>
		private int activationID;

		/// <summary>
		/// A reference to the particle system to control the
		/// animation of the edge.
		/// </summary>
		private ParticleSystem refParticleSystem;

		/// <summary>
		/// A reference to the MainModule of the particle system.
		/// It is used to set the start lifetime of the particle system.
		/// </summary>
		private ParticleSystem.MainModule mainModule;

		/// <summary>
		/// A reference to the EmissionModule of the particle system.
		/// It is used to set the emission rate of the particle system.
		/// </summary>
		private ParticleSystem.EmissionModule emissionModule;

		/// <summary>
		/// A reference to the ShapeModule of the particle system.
		/// It is used to set the scale of the particle system.
		/// </summary>
		private ParticleSystem.ShapeModule shapeModule;

		/// <summary>
		/// A reference to the pathway this node belongs to.
		/// </summary>
		private IGraphGO refMasterPathway;

		protected override void Awake()
		{
			base.Awake();
			triggerKOActions.left.action.performed += ManageKnockout;
			triggerKOActions.right.action.performed += ManageKnockout;

			activationID = Shader.PropertyToID("_Activation");
			refParticleSystem = GetComponentInChildren<ParticleSystem>();
			mainModule = refParticleSystem.main;
			emissionModule = refParticleSystem.emission;
			shapeModule = refParticleSystem.shape;
		}

		public override void OnDestroy()
		{
			triggerKOActions.left.action.performed -= ManageKnockout;
			triggerKOActions.right.action.performed -= ManageKnockout;
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			fluxLevel.OnValueChanged += ApplyFLChanges;
			fluxLevelClamped.OnValueChanged += ApplyFLCChanges;
			knockedOut.OnValueChanged += ApplyKOChanges;
		}

		public override void OnNetworkDespawn()
		{
			base.OnNetworkDespawn();

			fluxLevel.OnValueChanged -= ApplyFLChanges;
			fluxLevelClamped.OnValueChanged -= ApplyFLCChanges;
			knockedOut.OnValueChanged -= ApplyKOChanges;
		}

		/// <summary>
		/// Notifies the server that the edge should be activated.
		/// </summary>
		[ServerRpc(RequireOwnership = false)]
		private void ActivateServerRpc()
		{
			knockedOut.Value = false;
		}

		/// <inheritdoc/>
		protected override void ApplyCurrentColorChange(Color _previous, Color _current)
		{
			if (forceDefaultColor)
			{
				mpb.SetVector(colorID, Color.white);
			}
			else
			{
				mpb.SetVector(colorID, _current);
			}
			m_LineRenderer.SetPropertyBlock(mpb);
		}

        /// <summary>
        /// Callback function to react to the OnValueChanged event of <see cref="fluxLevel"/>.
        /// </summary>
        /// <param name="_previous">
        /// The previous value of <see cref="fluxLevel"/>. This is required for the
        /// to satisfy the signature constraint of the event but we don't use it.
        /// </param>
        /// <param name="_current">
        /// The newly set value of <see cref="fluxLevel"/>. This is required for the
        /// to satisfy the signature constraint of the event but we don't use it.
        /// </param>
        private void ApplyFLChanges(float _previous, float _current)
		{
			ApplyFluxLevel();
		}

		/// <summary>
		/// Callback function to react to the OnValueChanged event of
		/// <see cref="fluxLevelClamped"/>.
		/// </summary>
		/// <param name="_previous">
		/// The previous value of <see cref="fluxLevelClamped"/>. This is required for the
		/// to satisfy the signature constraint of the event but we don't use it.
		/// </param>
		/// <param name="_current">
		/// The newly set value of <see cref="fluxLevelClamped"/>. This is required for the
		/// to satisfy the signature constraint of the event but we don't use it.
		/// </param>
		private void ApplyFLCChanges(float _previous, float _current)
		{
			ApplyFluxLevelClamped();
		}

		/// <summary>
		/// Callback function to react to the OnValueChanged event of <see cref="knockedOut"/>.
		/// </summary>
		/// <param name="_previous">
		/// The previous value of <see cref="knockedOut"/>. This is required for the
		/// to satisfy the signature constraint of the event but we don't use it.
		/// </param>
		/// <param name="_current">
		/// The newly set value of <see cref="knockedOut"/>. This is required for the
		/// to satisfy the signature constraint of the event but we don't use it.
		/// </param>
		private void ApplyKOChanges(bool _previous, bool _current)
		{
			SetInformationString();
			mpb.SetFloat(activationID, knockedOut.Value? 0 : 1);
			m_LineRenderer.SetPropertyBlock(mpb);
		}

		/// <summary>
		/// Uses the information from <paramref name="_edge"/> to initialize the edge.
		/// </summary>
		/// <param name="_masterPathway">
		/// The pathway this edge belongs to.
		/// </param>
		/// <param name="_edge">
		/// The edge data to use to initialize the edge.
		/// </param>
		public void Initialize(CyJsonModule _masterPathway, IEdge _edge)
		{
#if UNITY_EDITOR
			m_LineRenderer = GetComponent<LineRenderer>();
			if (nameTextFieldContainer != null)
			{
				nameField = nameTextFieldContainer?.GetComponentInChildren<TextMeshProUGUI>();
			}

			mpb = new MaterialPropertyBlock();
			colorID = Shader.PropertyToID("_Color");
			mpb.SetVector(colorID, defaultColor);
			m_LineRenderer.SetPropertyBlock(mpb);

			activationID = Shader.PropertyToID("_Activation");
			refParticleSystem = GetComponentInChildren<ParticleSystem>();
			mainModule = refParticleSystem.main;
			emissionModule = refParticleSystem.emission;
			shapeModule = refParticleSystem.shape;
#endif
			refMasterPathway = _masterPathway;
			InstantiateInfoTags(new string[] { "" });
			SetEdgeData(_edge);
			gameObject.SetActive(true);
			gameObject.name = $"{edgeData.ID}";
			SetName(edgeData.reaction_name);
			HideName();
			SetDefaultWidth(1 / refMasterPathway.graphScalingData.sizeScaleFactor,
							1 / refMasterPathway.graphScalingData.sizeScaleFactor);

			SetLineRendererWidth();

			Transform start = refMasterPathway.DataID_to_DataGO[edgeData.source].transform;
			Transform target = refMasterPathway.DataID_to_DataGO[edgeData.target].transform;
			SetLineRendererPosition(start, target);
			SetCollider(start, target);

			//Particle system parameters
			emissionModule.rateOverTime = 0;
			shapeModule.scale = m_refBoxColliderHolder.transform.localScale;
			refParticleSystem.transform.position = start.position;
			refParticleSystem.transform.LookAt(target.position);
			mainModule.startLifetime = Vector3.Distance(start.position, target.position);
				

			m_nameTextFieldContainer.transform.position = 0.5f * (start.position + target.position) +
															1 / refMasterPathway.graphScalingData.sizeScaleFactor * 1.5f * Vector3.up;
		}

		/// <summary>
		/// Notifies the server that the edge should be knocked out.
		/// </summary>
		[ServerRpc(RequireOwnership = false)]
		private void KnockoutServerRpc()
		{
			knockedOut.Value = true;
		}

		/// <summary>
		/// The public interface to call back on Unity Events to knockout
		/// or activate a reaction represented by the edge.
		/// </summary>
		/// <remarks>Typically called back when the user presses a button
		/// while pointing at the edge.</remarks>
		public void ManageKnockout(InputAction.CallbackContext _ctx)
		{
			if (isFocused)
			{
				if (knockedOut.Value)
				{
					Activate();
				}

				else
				{
					Knockout();
				}
			}
		}

		/// <summary>
		/// The utility function to update the information string.
		/// </summary>
		private void SetInformationString()
		{
			informationString = $"SUID: {edgeData.ID} \n" +
								$"Name: {edgeData.name} \n" +
								$"Reaction: {edgeData.reaction_name} \n" +
								$"Knockedout: {knockedOut.Value} \n" +
								$"Flux: {fluxLevel.Value}";
			m_refInfoTagsContainer.transform.GetChild(0).GetComponent<InfoDisplayManager>().SetText(informationString);
		}

		/// <summary>
		/// Notifies the server of new values for <see cref="fluxLevel"/> and
		/// <see cref="fluxLevelClamped"/>.
		/// </summary>
		/// <param name="_fluxValue">
		/// The new value for <see cref="fluxLevel"/>.
		/// </param>
		/// <param name="_fluxClampedValue">
		/// The new value for <see cref="fluxLevelClamped"/>.
		/// </param>
		[ServerRpc(RequireOwnership = false)]
		private void SetFluxValuesServerRpc(float _fluxValue, float _fluxClampedValue)
		{
			fluxLevel.Value = _fluxValue;
			fluxLevelClamped.Value = _fluxClampedValue;
		}

		/// <summary>
		/// Sets the X and Y scale of the box collider relatively to the line renderer's width.
		/// </summary>
		private void SetColliderHeightWidth()
		{
			m_refBoxColliderHolder.transform.localScale = new Vector3(
															0.33f * Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),
															0.33f * Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),
															m_refBoxColliderHolder.transform.localScale.z);
		}

		/// <summary>
		/// Sets the value for <see cref="refMasterPathway"/>.
		/// </summary>
		/// <param name="_masterPathway">The value for <see cref="refMasterPathway"/>.</param>
		public void SetRefMasterPathway(IGraphGO _masterPathway)
		{
			refMasterPathway = _masterPathway;
		}

		/// <summary>
		/// Spreads the activation state to every contiguous downstream edge that are part of the same reaction.
		/// </summary>
		public void SpreadActivationDownward()
		{
			ActivateServerRpc();

			GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
			foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadActivationDownward();
				}
			}

			foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
					neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadActivationUpward();
				}
			}
		}

		/// <summary>
		/// Spreads the activation state to every contiguous upstream edge that are part of the same reaction.
		/// </summary>
		public void SpreadActivationUpward()
		{
			ActivateServerRpc();

			GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
			foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadActivationUpward();
				}
			}

			foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
					neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadActivationDownward();
				}
			}
		}

		/// <summary>
		/// Spreads the Knockout state to every contiguous downstream edge that are part of the same reaction.
		/// </summary>
		public void SpreadKODownward()
		{
			KnockoutServerRpc();

			GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
			foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadKODownward();
				}
			}

			foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
					neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadKOUpward();
				}
			}
		}

		/// <summary>
		/// Spreads the Knockout state to every contiguous upstream edge that are part of the same reaction.
		/// </summary>
		public void SpreadKOUpward()
		{
			KnockoutServerRpc();

			GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
			foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadKOUpward();
				}
			}

			foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
					neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadKODownward();
				}
			}
		}

		/// <summary>
		/// Spreads the highlighted state to every contiguous downstream edge that are part of the same reaction.
		/// </summary>
		public void SpreadHighlightDownward()
		{
			SetCurrentColorToHighlightServerRpc();
			refParticleSystem.gameObject.SetActive(true);
			refParticleSystem.Play();

			GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
			foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadHighlightDownward();
				}
			}

			foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
					neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadHighlightUpward();
				}
			}
		}

		/// <summary>
		/// Spreads the highlighted state to every contiguous upstream edge that are part of the same reaction.
		/// </summary>
		public void SpreadHighlightUpward()
		{
			SetCurrentColorToHighlightServerRpc();
			refParticleSystem.gameObject.SetActive(true);
			refParticleSystem.Play();

			GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
			foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadHighlightUpward();
				}
			}

			foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
					neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadHighlightDownward();
				}
			}
		}

		/// <summary>
		/// Spreads the unhighlighted state to every contiguous upstream edge that are part of the same reaction.
		/// </summary>
		public void SpreadUnsetHighlightDownward()
		{
			SetCurrentColorToDefaultServerRpc();
			refParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

			GameObject targetNode = refMasterPathway.DataID_to_DataGO[edgeData.target];
			foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadUnsetHighlightDownward();
				}
			}

			foreach (uint edgeID in targetNode.GetComponent<NodeGO>().nodeData.incommingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
					neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadUnsetHighlightUpward();
				}
			}
		}

		/// <summary>
		/// Spreads the unhighlighted state to every contiguous downstream edge that are part of the same reaction.
		/// </summary>
		public void SpreadUnsetHighlightUpward()
		{
			SetCurrentColorToDefaultServerRpc();
			refParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

			GameObject sourceNode = refMasterPathway.DataID_to_DataGO[edgeData.source];
			foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.incommingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadUnsetHighlightUpward();
				}
			}

			foreach (uint edgeID in sourceNode.GetComponent<NodeGO>().nodeData.outgoingEdges)
			{
				EdgeGO neighbourEdgeGo = refMasterPathway.DataID_to_DataGO[edgeID].GetComponent<EdgeGO>();
				if (neighbourEdgeGo.edgeData.ID != edgeData.ID &&
					neighbourEdgeGo.edgeData.name == edgeData.name)
				{
					neighbourEdgeGo.SpreadUnsetHighlightDownward();
				}
			}
		}

		#region - IColorHighlightable Methods -
		/// <inheritdoc/>
		public override void ApplyColor(Color _color)
		{
			mpb.SetVector(colorID, _color);
			if (m_LineRenderer != null)
			{
				m_LineRenderer.SetPropertyBlock(mpb);
			}
		}

		/// <inheritdoc/>
		public override void SetHighlight()
		{
			SpreadHighlightUpward();
			SpreadHighlightDownward();
		}
		
		/// <inheritdoc/>
		public override void UnsetHighlight()
		{
			if (!forceHighlight)
			{
				SpreadUnsetHighlightUpward();
				SpreadUnsetHighlightDownward();
			}
		}
		#endregion

		#region - IDive Methods -
		/// <inheritdoc/>
		public override IEnumerator GenerativeDiveInC()
		{
			Debug.LogError("GenerativeDiveInC not implemented for EdgeGO");
			yield return null;
		}
		#endregion

		#region - IEdgeGO Methods- 
		/// <inheritdoc/>
		public void ReverseOrientation()
		{
			Vector3 startBuffer = m_LineRenderer.GetPosition(0);
			m_LineRenderer.SetPosition(0, m_LineRenderer.GetPosition(1));
			m_LineRenderer.SetPosition(1, startBuffer);
			refParticleSystem.transform.localPosition = m_LineRenderer.GetPosition(0);
			refParticleSystem.transform.LookAt(m_LineRenderer.GetPosition(1));
		}

		/// <inheritdoc/>
		public void SetDefaultWidth(float _start, float _end)
		{
			//Debug.Log($"Set sw {defaultStartWidth}, ew {endWidthFactor}");
			defaultStartWidth = _start;
			defaultEndWidth = _end;
		}

		/// <inheritdoc/>
		public void SetEdgeData(IEdge _IEdge)
		{
			edgeData = _IEdge;
			SetInformationString();
		}

		/// <inheritdoc/>
		public void SetCollider(Transform _start, Transform _end)
		{
			m_refBoxColliderHolder.transform.localPosition = 0.5f * (_start.localPosition + _end.localPosition);
			m_refBoxColliderHolder.transform.LookAt(_end);
			m_refBoxColliderHolder.transform.localScale = new Vector3(
															0.33f * Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),//0.33f is custom for the inner size of the arrow texture
															0.33f * Mathf.Max(m_LineRenderer.startWidth, m_LineRenderer.endWidth),//0.33f is custom for the inner size of the arrow texture
															0.95f * Vector3.Distance(_start.localPosition, _end.localPosition));//0.95f is custom to avoid overlapping of the edge box collider with the nodes colliders
		}

		/// <inheritdoc/>
		public void SetLineRendererWidth()
		{
			if (forceDefaultWidth)
			{
				m_LineRenderer.startWidth = startWidthFactor * defaultStartWidth;
				m_LineRenderer.endWidth = endWidthFactor * defaultEndWidth;
			}
			else
			{
				m_LineRenderer.startWidth = startWidthFactor * Mathf.Max(defaultStartWidth, defaultStartWidth*fluxLevelClamped.Value);
				m_LineRenderer.endWidth = endWidthFactor * Mathf.Max(defaultEndWidth, defaultEndWidth*fluxLevelClamped.Value);
			}
		}

		/// <inheritdoc/>
		public void SetLineRendererPosition(Transform _start, Transform _end)
		{
			m_LineRenderer.SetPosition(0, _start.localPosition);
			m_LineRenderer.SetPosition(1, _end.localPosition);
		}
		#endregion

		#region - IMlprDataExchange Methods -
		/// <inheritdoc/>
		public override void AssembleFragmentedData()
		{
			Debug.LogError("AssembleFragmentedData not implemented for EdgeGO");
		}
		#endregion

		#region - IModulateFlux Methods- 

		/// <inheritdoc/>
		public void Activate()
		{
			SpreadActivationDownward();
			SpreadActivationUpward();
		}

		/// <inheritdoc/>
		public void Knockout()
		{
			SpreadKODownward();
			SpreadKOUpward();
		}

		/// <inheritdoc/>
		public void ApplyFluxLevel()
		{
			//Update emission rate
			emissionModule.rateOverTime = fluxLevel.Value;
		}

		/// <inheritdoc/>
		public void ApplyFluxLevelClamped()
		{
			SetInformationString();

			SetLineRendererWidth();

			SetColliderHeightWidth();
			shapeModule.scale = m_refBoxColliderHolder.transform.localScale;
		}

		/// <inheritdoc/>
		public void SetFlux(float _level, float _levelClamped)
		{
			SetFluxValuesServerRpc(_level, _levelClamped);
		}
		#endregion
	}
}
