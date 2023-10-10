using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.SceneManagement;
using ECellDive.UI;
using ECellDive.Utility;

namespace ECellDive
{
	namespace Modules
	{
		/// <summary>
		/// Base class holding references and methods used to manipulate
		/// the game object representation of a module.
		/// </summary>
		public abstract class GameNetModule : NetworkBehaviour,
									IDive,
									IFocus,
									IGroupable,
									IColorHighlightableNet,
									IInfoTags,
									INamed,
									IMlprData,
									IMlprVisibility
		{
			/// <summary>
			/// The collider of the module.
			/// </summary>
			[Tooltip("If null, tries to find one in the children of this gameobject.")]
			[SerializeField] protected Collider m_Collider;

			/// <summary>
			/// The renderer of the module.
			/// </summary>
			/// <remarks>
			/// May be null is the module doesn't have a renderer.
			/// </remarks>
			[Tooltip("If null, tries to find one in this gameobject.")]
			[SerializeField] protected Renderer m_Renderer;

			/// <summary>
			/// The line renderer of the module.
			/// </summary>
			/// <remarks>
			/// May be null is the module doesn't have a line renderer.
			/// </remarks>
			[Tooltip("If null, tries to find one in this gameobject.")]
			[SerializeField] protected LineRenderer m_LineRenderer;

			/// <summary>
			/// The material property block used to change the color of the module while 
			/// avoiding to create a new material instance.
			/// </summary>
			protected MaterialPropertyBlock mpb;

			/// <summary>
			/// The ID of the color property in the shader of the module to change its color
			/// in the material property block.
			/// </summary>
			protected int colorID;

			#region - IDive Members -
			/// <inheritdoc/>
			private bool m_isDiving = false;

			/// <inheritdoc/>
			public bool isDiving
			{
				get => m_isDiving;
				set => m_isDiving = value;
			}
			/// <summary>
			/// The field for the property <see cref="rootSceneId"/>.
			/// </summary>
			private NetworkVariable<int> m_rootSceneId = new NetworkVariable<int>();

			/// <inheritdoc/>
			public NetworkVariable<int> rootSceneId
			{
				get => m_rootSceneId;
				set => m_rootSceneId = value;
			}

			/// <summary>
			/// The field for the property <see cref="targetSceneId"/>.
			/// </summary>
			private NetworkVariable<int> m_targetSceneId = new NetworkVariable<int>();

			/// <inheritdoc/>
			public NetworkVariable<int> targetSceneId
			{
				get => m_targetSceneId;
				set => m_targetSceneId = value;
			}

			/// <summary>
			/// The field for the property <see cref="isReadyForDive"/>.
			/// </summary>
			private NetworkVariable<bool> m_isReadyForDive = new NetworkVariable<bool>(false,
				NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

			/// <inheritdoc/>
			public NetworkVariable<bool> isReadyForDive
			{
				get => m_isReadyForDive;
				set => m_isReadyForDive = value;
			}

			#endregion

			#region - IFocus Members -

			/// <summary>
			/// The field for the property <see cref="isFocused"/>.
			/// </summary>
			private bool m_isFocused = false;

			/// <inheritdoc/>
			public bool isFocused
			{
				get => m_isFocused;
				set => m_isFocused = value;
			}

			#endregion

			#region - IGroupable Members -
			/// <summary>
			/// The field for the property <see cref="grpMemberIndex"/>.
			/// </summary>
			private int m_grpMemberIndex = -1;

			/// <inheritdoc/>
			public int grpMemberIndex
			{
				get => m_grpMemberIndex;
				set => m_grpMemberIndex = value;
			}

            /// <summary>
            /// The field for the property <see cref="delegateTarget"/>.
            /// </summary>
            [SerializeField] private GameObject m_delegateTarget = null;

			///<inheritdoc/>
			public GameObject delegateTarget
			{
				get => m_delegateTarget;
				private set => m_delegateTarget = value;
			}
			#endregion

			#region - IColorHighlightableNet Members - 
			/// <summary>
			/// The field for the property <see cref="forceHighlight"/>.
			/// </summary>
			private bool m_forceHighlight = false;

			/// <inheritdoc/>
			public bool forceHighlight
			{
				get => m_forceHighlight;
				set => m_forceHighlight = value;
			}

			/// <summary>
			/// The field for the property <see cref="currentColor"/>.
			/// </summary>
			[SerializeField] private NetworkVariable<Color> m_currentColor;

			/// <inheritdoc/>
			public NetworkVariable<Color> currentColor
			{
				get => m_currentColor;
				set => m_currentColor = value;
			}

			/// <summary>
			/// The field for the property <see cref="defaultColor"/>.
			/// </summary>
			[SerializeField] private Color m_defaultColor;

			/// <inheritdoc/>
			public Color defaultColor
			{
				get => m_defaultColor;
				set => m_defaultColor = value;
			}

			/// <summary>
			/// The field for the property <see cref="highlightColor"/>.
			/// </summary>
			[SerializeField] private Color m_highlightColor;

			/// <inheritdoc/>
			public Color highlightColor
			{
				get => m_highlightColor;
				set => m_highlightColor = value;
			}
			#endregion

			#region - IInfoTags Members -

			/// <inheritdoc/>
			public bool areVisible { get; set; }

			/// <summary>
			/// The field for the property <see cref="displayInfoTagsActions"/>.
			/// </summary>
			[Header("Info Tags Data")]
			public LeftRightData<InputActionReference> m_displayInfoTagsActions;

			/// <inheritdoc/>
			public LeftRightData<InputActionReference> displayInfoTagsActions
			{
				get => m_displayInfoTagsActions;
				set => m_displayInfoTagsActions = value;
			}

			/// <summary>
			/// The field for the property <see cref="refInfoTagPrefab"/>.
			/// </summary>
			public GameObject m_refInfoTagPrefab;

			/// <inheritdoc/>
			public GameObject refInfoTagPrefab
			{
				get => m_refInfoTagPrefab;
				set => m_refInfoTagPrefab = value;
			}

			/// <summary>
			/// The field for the property <see cref="refInfoTagsContainer"/>.
			/// </summary>
			public GameObject m_refInfoTagsContainer;

			/// <inheritdoc/>
			public GameObject refInfoTagsContainer
			{
				get => m_refInfoTagsContainer;
				set => m_refInfoTagsContainer = value;
			}
			#endregion

			#region - INamed Members -
			/// <summary>
			/// The field for the property <see cref="nameTextFieldContainer"/>.
			/// </summary>
			[SerializeField] protected GameObject m_nameTextFieldContainer;

			/// <inheritdoc/>
			public GameObject nameTextFieldContainer
			{
				get => m_nameTextFieldContainer;
				protected set => m_nameTextFieldContainer = value;
			}

			/// <inheritdoc/>
			public TextMeshProUGUI nameField
			{
				get;
				protected set;
			}
			#endregion

			#region - IMlprData Members -

			/// <summary>
			/// The field for the property <see cref="fragmentedSourceData"/>.
			/// </summary>
			private List<byte[]> m_fragmentedSourceData = new List<byte[]>();

			/// <inheritdoc/>
			public List<byte[]> fragmentedSourceData
			{
				get => m_fragmentedSourceData;
				protected set => m_fragmentedSourceData = value;
			}

			/// <inheritdoc/>
			public byte[] sourceDataName
			{
				get;
				protected set;
			}

			/// <summary>
			/// The field for the property <see cref="sourceDataNbFrags"/>.
			/// </summary>
			private int m_sourceDataNbFrags = 0;

			/// <inheritdoc/>
			public int sourceDataNbFrags
			{
				get => m_sourceDataNbFrags;
				protected set => m_sourceDataNbFrags = value;
			}

			/// <summary>
			/// The field for the property <see cref="sourceDataNbFrags"/>.
			/// </summary>
			private NetworkVariable<int> m_nbClientReadyLoaded = new NetworkVariable<int>(0,
				NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

			/// <inheritdoc/>
			public NetworkVariable<int> nbClientReadyLoaded
			{
				get => m_nbClientReadyLoaded;
				protected set => m_nbClientReadyLoaded = value;
			}

			/// <summary>
			/// The field for the property <see cref="isReadyForGeneration"/>.
			/// </summary>
			private NetworkVariable<bool> m_isReadyForGeneration = new NetworkVariable<bool>(false,
				NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

			/// <inheritdoc/>
			public NetworkVariable<bool> isReadyForGeneration
			{
				get => m_isReadyForGeneration;
				protected set => m_isReadyForGeneration = value;
			}

			#endregion

			#region - IMlrpVisibility Members -

			/// <summary>
			/// The field for the property <see cref="isActivated"/>.
			/// </summary>
			private NetworkVariable<bool> m_isActivated = new NetworkVariable<bool>(true);

			/// <inheritdoc/>
			public NetworkVariable<bool> isActivated
			{
				get => m_isActivated;
				protected set => m_isActivated = value;
			}
			#endregion

			protected virtual void Awake()
			{
				areVisible = false;

				m_displayInfoTagsActions.left.action.performed += ManageInfoTagsDisplay;
				m_displayInfoTagsActions.right.action.performed += ManageInfoTagsDisplay;

				if (m_Collider == null)
				{
					m_Collider = GetComponentInChildren<Collider>();
				}
				if (m_Renderer == null)
				{
					m_Renderer = GetComponent<Renderer>();
				}
				if(m_LineRenderer == null)
				{
					m_LineRenderer = GetComponent<LineRenderer>();
				}
			}

			public override void OnDestroy()
			{
				m_displayInfoTagsActions.left.action.performed -= ManageInfoTagsDisplay;
				m_displayInfoTagsActions.right.action.performed -= ManageInfoTagsDisplay;

				m_currentColor.OnValueChanged -= ApplyCurrentColorChange;
				isActivated.OnValueChanged -= ManageActivationStatus;
			}

			public override void OnNetworkSpawn()
			{
				mpb = new MaterialPropertyBlock();
				colorID = Shader.PropertyToID("_Color");
				m_currentColor.OnValueChanged += ApplyCurrentColorChange;
				isActivated.OnValueChanged += ManageActivationStatus;
				SetCurrentColorToDefaultServerRpc();

				if (nameTextFieldContainer != null)
				{
					nameTextFieldContainer.SetActive(true);
					nameField = nameTextFieldContainer.GetComponentInChildren<TextMeshProUGUI>();
				}

				//If the target has not been set in the editor to a specific gameobject,
				//we set it to this game object by default.
				if (delegateTarget == null)
				{
					delegateTarget = gameObject;
				}
			}

			/// <summary>
			/// Proxy function to be usable within NetworkVariable's OnValueChanged callback.
			/// </summary>
			/// <param name="_previous">
			/// The previous color value of the NetworkVariable.
			/// </param>
			/// <param name="_current">
			/// The current color value of the NetworkVariable.
			/// </param>
			/// <remarks>
			/// The <paramref name="_previous"/> and <paramref name="_current"/> parameters are
			/// there only to match the signature but the value of <paramref name="_previous"/>
			/// is not used.
			/// </remarks>
			protected virtual void ApplyCurrentColorChange(Color _previous, Color _current)
			{
				ApplyColor(_current);
			}

			/// <summary>
			/// Will rotate the module to face the active camera.
			/// </summary>
			/// <remarks>Callback set in the editor</remarks>
			public void LookAt()
			{
				GetComponent<ILookAt>().LookAt();
			}

			/// <summary>
			/// Input call back action on which the floating display
			/// is turned on or off.
			/// </summary>
			/// <param name="_ctx">The input action callback context</param>
			private void ManageInfoTagsDisplay(InputAction.CallbackContext _ctx)
			{
				if (isFocused)
				{
					areVisible = !areVisible;
					if (areVisible)
					{
						DisplayInfoTags();
					}
					else
					{
						HideInfoTags();
					}
				}
			}

			/// <summary>
			/// The method to call when we wish the server to destroy a GameNetModule.
			/// </summary>
			[ServerRpc(RequireOwnership = false)]
			public void SelfDestroyServerRpc()
			{
				if (targetSceneId != null)//if a dive scene has been generated for the data module
				{
					if (!DiveScenesManager.Instance.CheckIfDiveSceneHasPlayers(targetSceneId.Value))
					{
						DiveScenesManager.Instance.DestroyDiveScene(targetSceneId.Value);
						Destroy(gameObject);
					}
					else
					{
						LogSystem.AddMessage(LogMessageTypes.Errors,
							"You are trying to destroy a dive scene while divers are inside.");
					}
				}
				else
				{
					Destroy(gameObject);
				}
			}

			#region - IDive Methods -
			/// <inheritdoc/>
			public void DirectDiveIn()
			{
				StartCoroutine(DirectDiveInC());
			}

			/// <inheritdoc/>
			public IEnumerator DirectDiveInC()
			{
				DiveScenesManager.Instance.SwitchingScenesServerRpc(rootSceneId.Value,
																	targetSceneId.Value,
																	NetworkManager.Singleton.LocalClientId);
				//Wait until the client has switched to the target scene
				yield return new WaitUntil(DiveScenesManager.Instance.SceneSwitchIsFinished);

				isDiving = false;
			}

			/// <inheritdoc/>
			public void GenerativeDiveIn()
			{
				StartCoroutine(GenerativeDiveInC());
			}

			/// <inheritdoc/>
			public abstract IEnumerator GenerativeDiveInC();

			/// <inheritdoc/>
			public void TryDiveIn()
			{
				if (isReadyForGeneration.Value)
				{
					isDiving = true;
					if (isReadyForDive.Value)
					{
						DirectDiveIn();
					}
					else
					{
						GenerativeDiveIn();
					}
				}
			}

			#endregion

			#region - IFocus Methods -
			/// <inheritdoc/>
			public void SetFocus()
			{
				m_isFocused = true;
			}
			/// <inheritdoc/>
			public void UnsetFocus()
			{
				m_isFocused = false;
			}
			#endregion

			#region - IColorHighlightable Methods -

			public virtual void ApplyColor(Color _color)
			{
				mpb.SetVector(colorID, _color);
				if (m_Renderer != null)
				{
					m_Renderer.SetPropertyBlock(mpb);
				}
			}

			/// <inheritdoc/>
			[ServerRpc(RequireOwnership = false)]
			public void SetCurrentColorToDefaultServerRpc()
			{
				m_currentColor.Value = m_defaultColor;
			}

			/// <inheritdoc/>
			[ServerRpc(RequireOwnership = false)]
			public virtual void SetCurrentColorToHighlightServerRpc()
			{
				m_currentColor.Value = m_highlightColor;
			}

			/// <inheritdoc/>
			public virtual void SetHighlight()
			{
				SetCurrentColorToHighlightServerRpc();
			}

			/// <inheritdoc/>
			public virtual void UnsetHighlight()
			{
				if (!m_forceHighlight)
				{
					SetCurrentColorToDefaultServerRpc();
				}
			}
			#endregion

			#region - IInfoTags Methods -
			/// <inheritdoc/>
			public void DisplayInfoTags()
			{
				if (refInfoTagsContainer != null)
				{
					foreach (Transform _infoTag in refInfoTagsContainer.transform)
					{
						_infoTag.gameObject.SetActive(true);
					}
				}
			}

			/// <inheritdoc/>
			public void HideInfoTags()
			{
				if (refInfoTagsContainer != null)
				{
					foreach (Transform _infoTag in refInfoTagsContainer.transform)
					{
						_infoTag.gameObject.SetActive(false);
					}
				}
			}

			/// <inheritdoc/>
			public void InstantiateInfoTag(Vector2 _xyPosition, string _content)
			{
				GameObject infoTag = Instantiate(refInfoTagPrefab, refInfoTagsContainer.transform);
				infoTag.transform.localPosition = new Vector3(_xyPosition.x, _xyPosition.y, 0f);
				infoTag.GetComponent<InfoDisplayManager>().SetText(_content);
			}

			/// <inheritdoc/>
			public void InstantiateInfoTags(string[] _content)
			{
				float angle = 360 / _content.Length;
				float radius = 1.25f * Mathf.Max(new float[]{transform.localScale.x,
															 transform.localScale.y,
															 transform.localScale.z });

				for (int i = 0; i < _content.Length; i++)
				{
					Vector2 xyPosition = Positioning.RadialPosition(radius, i * angle);
					InstantiateInfoTag(xyPosition, _content[i]);
				}
			}

			/// <inheritdoc/>
			public void ShowInfoTags()
			{
				foreach (Transform _infoTag in refInfoTagsContainer.transform)
				{
					_infoTag.gameObject.GetComponent<ILookAt>().LookAt();
				}
			}
			#endregion

			#region - INamed Methods -
			/// <inheritdoc/>
			public virtual void DisplayName()
			{
				m_nameTextFieldContainer.gameObject.SetActive(true);
			}

			/// <inheritdoc/>
			public string GetName()
			{
				return nameField.text;
			}

			/// <inheritdoc/>
			public void HideName()
			{
				m_nameTextFieldContainer.gameObject.SetActive(false);
			}

			/// <inheritdoc/>
			public void SetName(string _name)
			{
				nameField.text = _name;
			}

			/// <inheritdoc/>
			public void ShowName()
			{
				m_nameTextFieldContainer.GetComponent<ILookAt>().LookAt();
			}
			#endregion

			#region - IMlprData Methods -
			///<inheritdoc/>
			public abstract void AssembleFragmentedData();

			///<inheritdoc/>
			public IEnumerator BroadcastSourceDataC()
			{
				yield return new WaitForEndOfFrame();

				BroadcastSourceDataNameServerRpc(sourceDataName);

				yield return new WaitForEndOfFrame();

				BroadcastSourceDataNbFragsServerRpc((ushort)sourceDataNbFrags);

				yield return new WaitForEndOfFrame();

				StartCoroutine(BroadcastSourceDataFragsC(fragmentedSourceData));
			}

			///<inheritdoc/>
			[ClientRpc]
			public void BroadcastSourceDataFragClientRpc(byte[] _fragment)
			{
				//LogSystem.AddMessage(LogMessageTypes.Debug,
				//        "Client receives boradcasted source data fragment.");
				if (IsOwner) return;

				fragmentedSourceData.Add(_fragment);
				if (fragmentedSourceData.Count == sourceDataNbFrags)
				{
					ConfirmSourceDataReceptionServerRpc();
					AssembleFragmentedData();
				}
			}

			///<inheritdoc/>
			[ClientRpc]
			public void BroadcastSourceDataNameClientRpc(byte[] _name)
			{
				LogSystem.AddMessage(LogMessageTypes.Debug,
						"Client receives boradcasted source data name.");
				if (IsOwner) return;

				sourceDataName = _name;
			}

			/// <inheritdoc/>
			[ClientRpc]
			public void BroadcastSourceDataNbFragsClientRpc(ushort _sourceDataNbFrags)
			{
				LogSystem.AddMessage(LogMessageTypes.Debug,
						"Client receives boradcasted source nb frags.");
				if (IsOwner) return;

				sourceDataNbFrags = _sourceDataNbFrags;
			}

			/// <inheritdoc/>
			[ServerRpc]
			public void BroadcastSourceDataFragServerRpc(byte[] _fragment)
			{
				//LogSystem.AddMessage(LogMessageTypes.Debug,
				//        "Server Sends 1 fragment.");
				BroadcastSourceDataFragClientRpc(_fragment);
			}

			/// <inheritdoc/>
			[ServerRpc]
			public void BroadcastSourceDataNameServerRpc(byte[] _name)
			{
				LogSystem.AddMessage(LogMessageTypes.Debug,
						"Server boradcasts source data name.");
				BroadcastSourceDataNameClientRpc(_name);
			}

			/// <inheritdoc/>
			[ServerRpc]
			public void BroadcastSourceDataNbFragsServerRpc(ushort _sourceDataNbFrags)
			{
				BroadcastSourceDataNbFragsClientRpc(_sourceDataNbFrags);
			}

			/// <inheritdoc/>
			public IEnumerator BroadcastSourceDataFragsC(List<byte[]> _fragmentedSourceData)
			{
				foreach (byte[] _frag in _fragmentedSourceData)
				{
					BroadcastSourceDataFragServerRpc(_frag);
					yield return new WaitForEndOfFrame();//waiting to avoid going over max network payload
				}
			}

			/// <inheritdoc/>
			[ServerRpc(RequireOwnership = false)]
			public void ConfirmSourceDataReceptionServerRpc()
			{
				LogSystem.AddMessage(LogMessageTypes.Debug,
						"A Client Confirms reception of all the fragments.");
				nbClientReadyLoaded.Value++;
				if (nbClientReadyLoaded.Value == NetworkManager.Singleton.ConnectedClientsIds.Count)
				{
					isReadyForGeneration.Value = true;
				}
			}

			/// <inheritdoc/>
			public void DirectReceiveSourceData(byte[] _sourceDataName, List<byte[]> _fragmentedSourceData)
			{
				LogSystem.AddMessage(LogMessageTypes.Debug,
						"The module received its local copy of the fragmented data.");
				fragmentedSourceData = _fragmentedSourceData;
				sourceDataName = _sourceDataName;
				sourceDataNbFrags = _fragmentedSourceData.Count;

				ConfirmSourceDataReceptionServerRpc();
				AssembleFragmentedData();

				StartCoroutine(BroadcastSourceDataC());
			}

			/// <inheritdoc/>
			[ServerRpc(RequireOwnership = false)]
			public virtual void RequestSourceDataGenerationServerRpc(ulong _expeditorClientID)
			{
				Debug.LogError("This method should be overriden in the child class.");
			}

			/// <inheritdoc/>
			public IEnumerator SendSourceDataC(ulong _targetClientID)
			{
				ClientRpcParams clientRpcParams = new ClientRpcParams
				{
					Send = new ClientRpcSendParams
					{
						TargetClientIds = new ulong[] { _targetClientID },
					}
				};

				SendSourceDataNameClientRpc(sourceDataName, clientRpcParams);

				yield return new WaitForEndOfFrame();

				SendSourceDataNbFragsClientRpc((ushort)sourceDataNbFrags, clientRpcParams);

				yield return new WaitForEndOfFrame();

				StartCoroutine(SendSourceDataFragsC(fragmentedSourceData, clientRpcParams));
			}

			/// <inheritdoc/>
			[ClientRpc]
			public void SendSourceDataFragClientRpc(byte[] _fragment, ClientRpcParams _clientRpcParams)
			{
				fragmentedSourceData.Add(_fragment);
				if (fragmentedSourceData.Count == sourceDataNbFrags)
				{
					ConfirmSourceDataReceptionServerRpc();
					AssembleFragmentedData();
				}
			}

			/// <inheritdoc/>
			[ClientRpc]
			public void SendSourceDataNameClientRpc(byte[] _name, ClientRpcParams _clientRpcParams)
			{
				LogSystem.AddMessage(LogMessageTypes.Debug,
						"Client receives source data name.");
				sourceDataName = _name;
			}

			/// <inheritdoc/>
			[ClientRpc]
			public void SendSourceDataNbFragsClientRpc(ushort _sourceDataNbFrags, ClientRpcParams _clientRpcParams)
			{
				LogSystem.AddMessage(LogMessageTypes.Debug,
						"Client receives source nb frags.");
				sourceDataNbFrags = _sourceDataNbFrags;
			}

			/// <inheritdoc/>
			public IEnumerator SendSourceDataFragsC(List<byte[]> _fragmentedSourceData, ClientRpcParams _clientRpcParams)
			{
				foreach (byte[] _frag in _fragmentedSourceData)
				{
					SendSourceDataFragClientRpc(_frag, _clientRpcParams);
					yield return new WaitForEndOfFrame();//waiting to avoid going over max network payload
				}
			}
			#endregion

			#region - IMlprVisibility Methods -
			/// <inheritdoc/>
			public virtual void ManageActivationStatus(bool _previous, bool _current)
			{
				gameObject.SetActive(isActivated.Value);
			}

			/// <inheritdoc/>
			public virtual void NetHide()
			{
				HideInfoTags();
				//Debug.Log("Try to Hide");
				if (m_Collider != null)
				{
					m_Collider.enabled = false;
				}

				if (m_Renderer != null)
				{
					//Debug.Log("Hiding m_Renderer");
					m_Renderer.enabled = false;
				}

				if (m_LineRenderer != null)
				{
					//Debug.Log("Hiding m_LineRenderer");
					m_LineRenderer.enabled = false;
				}
			}

			/// <inheritdoc/>
			[ClientRpc]
			public virtual void NetHideClientRpc(ClientRpcParams _clientRpcParams)
			{
				NetHide();
			}
			
			/// <inheritdoc/>
			public virtual void NetShow()
			{
				if (m_Collider != null)
				{
					m_Collider.enabled = true;
				}

				if (m_Renderer != null)
				{
					m_Renderer.enabled = true;
				}

				if (m_LineRenderer != null)
				{
					m_LineRenderer.enabled = true;
				}
			}

			/// <inheritdoc/>
			[ClientRpc]
			public virtual void NetShowClientRpc(ClientRpcParams _clientRpcParams)
			{
				NetShow();
			}

			/// <inheritdoc/>
			[ServerRpc(RequireOwnership = false)]
			public void RequestSetActiveServerRpc(bool _active)
			{
				isActivated.Value = _active;
			}
			#endregion
		}
	}
}
