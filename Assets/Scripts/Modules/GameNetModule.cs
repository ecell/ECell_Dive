using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Multiplayer;
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
        /// 
        public class GameNetModule : NetworkBehaviour,
                                    IDive,
                                    IFocus,
                                    IGroupable,
                                    IHighlightable,
                                    IInfoTags,
                                    INamed,
                                    IMlprData,
                                    IMlprVisibility
        {
            protected Collider m_Collider;
            protected Renderer m_Renderer;
            protected LineRenderer m_LineRenderer;
            protected MaterialPropertyBlock mpb;
            protected int colorID;

            #region - IDive Members -
            [SerializeField] private ControllersSymetricAction m_diveActions;
            public ControllersSymetricAction diveActions
            {
                get => m_diveActions;
                set
                {
                    m_diveActions = value;
                    m_diveActions.leftController = value.leftController;
                    m_diveActions.rightController = value.rightController;
                }
            }

            private NetworkVariable<int> m_rootSceneId = new NetworkVariable<int>();
            public NetworkVariable<int> rootSceneId
            {
                get => m_rootSceneId;
                set => m_rootSceneId = value;
            }

            private NetworkVariable<int> m_targetSceneId = new NetworkVariable<int>();

            public NetworkVariable<int> targetSceneId
            {
                get => m_targetSceneId;
                set => m_targetSceneId = value;
            }

            private NetworkVariable<bool> m_isReadyForDive = new NetworkVariable<bool>(false,
                NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            public NetworkVariable<bool> isReadyForDive
            {
                get => m_isReadyForDive;
                set => m_isReadyForDive = value;
            }

            #endregion

            #region - IFocus Members -
            private bool m_isFocused = false;
            public bool isFocused
            {
                get => m_isFocused;
                set => m_isFocused = value;
            }

            #endregion

            #region - IGroupable Members -
            private int m_grpMemberIndex = -1;
            public int grpMemberIndex
            {
                get => m_grpMemberIndex;
                set => m_grpMemberIndex = value;
            }
            #endregion

            #region - IHighlightable Members - 

            [SerializeField] private NetworkVariable<Color> m_currentColor;
            public NetworkVariable<Color> currentColor
            {
                get => m_currentColor;
                set => m_currentColor = value;
            }

            [SerializeField] private Color m_defaultColor;
            public Color defaultColor
            {
                get => m_defaultColor;
                set => m_defaultColor = value;
            }

            [SerializeField] private Color m_highlightColor;
            public Color highlightColor
            {
                get => m_highlightColor;
                set => m_highlightColor = value;
            }

            private bool m_forceHighlight = false;
            public bool forceHighlight
            {
                get => m_forceHighlight;
                set => m_forceHighlight = value;
            }

            #endregion

            #region - IInfoTags Members -
            public bool areVisible { get; set; }

            [Header("Info Tags Data")]
            public ControllersSymetricAction m_displayInfoTagsActions;
            public ControllersSymetricAction displayInfoTagsActions
            {
                get => m_displayInfoTagsActions;
                set => displayInfoTagsActions = m_displayInfoTagsActions;
            }
            public GameObject m_refInfoTagPrefab;
            public GameObject refInfoTagPrefab
            {
                get => m_refInfoTagPrefab;
                set => refInfoTagPrefab = m_refInfoTagPrefab;
            }
            public GameObject m_refInfoTagsContainer;
            public GameObject refInfoTagsContainer
            {
                get => m_refInfoTagsContainer;
                set => refInfoTagsContainer = m_refInfoTagsContainer;
            }

            public List<GameObject> m_refInfoTags;
            public List<GameObject> refInfoTags
            {
                get => m_refInfoTags;
                set => refInfoTags = m_refInfoTags;
            }
            #endregion

            #region - INamed Members -
            [SerializeField] protected GameObject m_nameTextFieldContainer;
            public GameObject nameTextFieldContainer
            {
                get => m_nameTextFieldContainer;
                protected set => m_nameTextFieldContainer = value;
            }

            public TextMeshProUGUI nameField
            {
                get;
                protected set;
            }
            #endregion

            #region - IMlprData Members -

            private List<byte[]> m_fragmentedSourceData = new List<byte[]>();
            public List<byte[]> fragmentedSourceData
            {
                get => m_fragmentedSourceData;
                protected set => m_fragmentedSourceData = value;
            }

            public byte[] sourceDataName
            {
                get;
                protected set;
            }

            private int m_sourceDataNbFrags = 0;
            public int sourceDataNbFrags
            {
                get => m_sourceDataNbFrags;
                protected set => m_sourceDataNbFrags = value;
            }

            private NetworkVariable<int> m_nbClientReadyLoaded = new NetworkVariable<int>(0,
                NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            public NetworkVariable<int> nbClientReadyLoaded
            {
                get => m_nbClientReadyLoaded;
                protected set => m_nbClientReadyLoaded = value;
            }

            private NetworkVariable<bool> m_isReadyForGeneration = new NetworkVariable<bool>(false,
                NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            public NetworkVariable<bool> isReadyForGeneration
            {
                get => m_isReadyForGeneration;
                protected set => m_isReadyForGeneration = value;
            }

            #endregion

            #region - IMlrpVisibility Members -
            private NetworkVariable<bool> m_isActivated = new NetworkVariable<bool>(true);
            public NetworkVariable<bool> isActivated
            {
                get => m_isActivated;
                protected set => m_isActivated = value;
            }
            #endregion

            protected virtual void Awake()
            {
                areVisible = false;

                diveActions.leftController.action.performed += TryDiveIn;
                diveActions.rightController.action.performed += TryDiveIn;

                m_displayInfoTagsActions.leftController.action.performed += ManageInfoTagsDisplay;
                m_displayInfoTagsActions.rightController.action.performed += ManageInfoTagsDisplay;

                //Debug.Log("Starting up " + gameObject.name);
                m_Collider = GetComponentInChildren<Collider>();
                m_Renderer = GetComponent<Renderer>();
                m_LineRenderer = GetComponent<LineRenderer>();

                if (nameTextFieldContainer != null)
                {
                    nameField = nameTextFieldContainer?.GetComponentInChildren<TextMeshProUGUI>();
                }
            }

            public override void OnDestroy()
            {
                diveActions.leftController.action.performed -= TryDiveIn;
                diveActions.rightController.action.performed -= TryDiveIn;

                m_displayInfoTagsActions.leftController.action.performed -= ManageInfoTagsDisplay;
                m_displayInfoTagsActions.rightController.action.performed -= ManageInfoTagsDisplay;

                currentColor.OnValueChanged -= ApplyCurrentColorChange;
                isActivated.OnValueChanged -= ManageActivationStatus;
            }

            public override void OnNetworkSpawn()
            {
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                currentColor.OnValueChanged += ApplyCurrentColorChange;
                isActivated.OnValueChanged += ManageActivationStatus;
                currentColor.Value = defaultColor;
            }

            protected virtual void ApplyCurrentColorChange(Color _previous, Color _current)
            {

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

            [ServerRpc(RequireOwnership = false)]
            private void SetCurrentColorServerRpc(Color _color)
            {
                currentColor.Value = _color;
            }

            #region - IDive Methods -

            public void DirectDiveIn()
            {
                StartCoroutine(DirectDiveInC());
            }

            public virtual IEnumerator DirectDiveInC()
            {
                //TODO: DIVE START ANIMATION
                yield return null;

                Debug.Log($"DirectDiveInC for netobj: {NetworkBehaviourId}");
                DiveScenesManager.Instance.SwitchingScenesServerRpc(rootSceneId.Value,
                                                                    targetSceneId.Value,
                                                                    NetworkManager.Singleton.LocalClientId);
                //TODO: DIVE END ANIMATION

            }

            public void GenerativeDiveIn()
            {
                StartCoroutine(GenerativeDiveInC());
            }

            public virtual IEnumerator GenerativeDiveInC()
            {
                //TODO: DATA GENERATION START ANIMATION

                Debug.LogError($"Generative dive in {gameObject.name}:{nameField.text} but no" +
                    $"custom behaviour has been defined for that type of module");
                yield return null;
                //TODO: DATA GENERATION END ANIMATION

            }

            public void TryDiveIn(InputAction.CallbackContext _ctx)
            {
                StartCoroutine(TryDiveInC());
            }

            public virtual IEnumerator TryDiveInC()
            {
                if (isFocused && isReadyForGeneration.Value)
                {
                    //Wait for animation to finish;
                    yield return null;
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
            public void SetFocus()
            {
                m_isFocused = true;
            }

            public void UnsetFocus()
            {
                m_isFocused = false;
            }
            #endregion

            #region - IHighlightable Methods -
            /// <inheritdoc/>
            [ServerRpc(RequireOwnership = false)]
            public void SetDefaultServerRpc()
            {
                m_currentColor.Value = m_defaultColor;
            }

            /// <inheritdoc/>
            public void SetDefaultColor(Color _c)
            {
                m_defaultColor = _c;
            }

            /// <inheritdoc/>
            [ServerRpc(RequireOwnership = false)]
            public virtual void SetHighlightServerRpc()
            {
                Debug.Log("Highlight");
                m_currentColor.Value = m_highlightColor;
            }

            /// <inheritdoc/>
            public void SetHighlightColor(Color _c)
            {
                m_highlightColor = _c;
            }

            /// <inheritdoc/>
            [ServerRpc(RequireOwnership = false)]
            public virtual void UnsetHighlightServerRpc()
            {
                if (!forceHighlight)
                {
                    m_currentColor.Value = m_defaultColor;
                }
            }
            #endregion

            #region - IInfoTags Methods -
            public void DisplayInfoTags()
            {
                foreach (GameObject _infoTag in refInfoTags)
                {
                    _infoTag.SetActive(true);
                }
            }

            public void HideInfoTags()
            {
                foreach (GameObject _infoTag in refInfoTags)
                {
                    _infoTag.SetActive(false);
                }
            }

            public void InstantiateInfoTag(Vector2 _xyPosition, string _content)
            {
                GameObject infoTag = Instantiate(refInfoTagPrefab, refInfoTagsContainer.transform);
                infoTag.transform.localPosition = new Vector3(_xyPosition.x, _xyPosition.y, 0f);
                infoTag.GetComponent<InfoDisplayManager>().SetText(_content);
                refInfoTags.Add(infoTag);
            }

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

            public void ShowInfoTags()
            {
                foreach (GameObject _infoTag in refInfoTags)
                {
                    _infoTag.GetComponent<InfoDisplayManager>().LookAt();
                }
            }
            #endregion

            #region - INamed Methods -

            public virtual void DisplayName()
            {
                m_nameTextFieldContainer.gameObject.SetActive(true);
                nameField.gameObject.SetActive(true);
            }

            public string GetName()
            {
                return nameField.text;
            }

            public void HideName()
            {
                m_nameTextFieldContainer.gameObject.SetActive(false);
                nameField.gameObject.SetActive(false);
            }

            public void SetName(string _name)
            {
                nameField.text = _name;
            }

            public void ShowName()
            {
                m_nameTextFieldContainer.GetComponent<ILookAt>().LookAt();
            }
            #endregion

            #region - IMlprData Methods -
            public virtual void AssembleFragmentedData()
            {
                //isReadyForGeneration = true;
            }

            public IEnumerator BroadcastSourceDataC()
            {
                yield return new WaitForEndOfFrame();

                BroadcastSourceDataNameServerRpc(sourceDataName);

                yield return new WaitForEndOfFrame();

                BroadcastSourceDataNbFragsServerRpc((ushort)sourceDataNbFrags);

                yield return new WaitForEndOfFrame();

                StartCoroutine(BroadcastSourceDataFragsC(fragmentedSourceData));
            }

            [ClientRpc]
            public void BroadcastSourceDataFragClientRpc(byte[] _fragment)
            {
                //LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                //        "Client receives boradcasted source data fragment.");
                if (IsOwner) return;

                fragmentedSourceData.Add(_fragment);
                if (fragmentedSourceData.Count == sourceDataNbFrags)
                {
                    ConfirmSourceDataReceptionServerRpc();
                    AssembleFragmentedData();
                }
            }

            [ClientRpc]
            public void BroadcastSourceDataNameClientRpc(byte[] _name)
            {
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        "Client receives boradcasted source data name.");
                if (IsOwner) return;

                sourceDataName = _name;
            }

            [ClientRpc]
            public void BroadcastSourceDataNbFragsClientRpc(ushort _sourceDataNbFrags)
            {
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        "Client receives boradcasted source nb frags.");
                if (IsOwner) return;

                sourceDataNbFrags = _sourceDataNbFrags;
            }

            [ServerRpc]
            public void BroadcastSourceDataFragServerRpc(byte[] _fragment)
            {
                //LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                //        "Server Sends 1 fragment.");
                BroadcastSourceDataFragClientRpc(_fragment);
            }

            [ServerRpc]
            public void BroadcastSourceDataNameServerRpc(byte[] _name)
            {
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        "Server boradcasts source data name.");
                BroadcastSourceDataNameClientRpc(_name);
            }

            [ServerRpc]
            public void BroadcastSourceDataNbFragsServerRpc(ushort _sourceDataNbFrags)
            {
                BroadcastSourceDataNbFragsClientRpc(_sourceDataNbFrags);
            }

            public IEnumerator BroadcastSourceDataFragsC(List<byte[]> _fragmentedSourceData)
            {
                foreach (byte[] _frag in _fragmentedSourceData)
                {
                    BroadcastSourceDataFragServerRpc(_frag);
                    yield return new WaitForEndOfFrame();//waiting to avoid going over max network payload
                }
            }

            [ServerRpc(RequireOwnership = false)]
            public void ConfirmSourceDataReceptionServerRpc()
            {
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        "A Client Confirms reception of all the fragments.");
                nbClientReadyLoaded.Value++;
                if (nbClientReadyLoaded.Value == NetworkManager.Singleton.ConnectedClientsIds.Count)
                {
                    isReadyForGeneration.Value = true;
                }
            }

            public void DirectReceiveSourceData(byte[] _sourceDataName, List<byte[]> _fragmentedSourceData)
            {
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        "The module received its local copy of the fragmented data.");
                fragmentedSourceData = _fragmentedSourceData;
                sourceDataName = _sourceDataName;
                sourceDataNbFrags = _fragmentedSourceData.Count;

                ConfirmSourceDataReceptionServerRpc();
                AssembleFragmentedData();

                StartCoroutine(BroadcastSourceDataC());
            }

            [ServerRpc(RequireOwnership = false)]
            public virtual void RequestSourceDataGenerationServerRpc(ulong _expeditorClientID)
            {
                Debug.LogError("No Generation scheme has been defined for this GameNetModule. " +
                    "Please, override this method and code how you the data stored in this module" +
                    " should be represented in the scene.");
            }

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

            [ClientRpc]
            public void SendSourceDataNameClientRpc(byte[] _name, ClientRpcParams _clientRpcParams)
            {
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        "Client receives source data name.");
                sourceDataName = _name;
            }

            [ClientRpc]
            public void SendSourceDataNbFragsClientRpc(ushort _sourceDataNbFrags, ClientRpcParams _clientRpcParams)
            {
                LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        "Client receives source nb frags.");
                sourceDataNbFrags = _sourceDataNbFrags;
            }

            public IEnumerator SendSourceDataFragsC(List<byte[]> _fragmentedSourceData, ClientRpcParams _clientRpcParams)
            {
                foreach (byte[] _frag in _fragmentedSourceData)
                {
                    SendSourceDataFragClientRpc(_frag, _clientRpcParams);
                    yield return new WaitForEndOfFrame();//waiting to avoid going over max network payload
                }
            }
            #endregion

            #region - IMlprVisibility -
            public virtual void ManageActivationStatus(bool _previous, bool _current)
            {
                gameObject.SetActive(isActivated.Value);
            }

            public virtual void NetHide()
            {
                HideInfoTags();
                //Debug.Log("Try to Hide");
                if (m_Collider != null)
                {
                    m_Collider.enabled = false;
                }
                
                if (nameField != null)
                {
                    nameField.gameObject.SetActive(false);
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

            [ClientRpc]
            public virtual void NetHideClientRpc(ClientRpcParams _clientRpcParams)
            {
                NetHide();
            }
            
            public virtual void NetShow()
            {
                //Debug.Log("Trying to Show");

                if (m_Collider != null)
                {
                    m_Collider.enabled = true;
                }

                if (nameField != null)
                {
                    nameField.gameObject.SetActive(true);
                }

                if (m_Renderer != null)
                {
                    //Debug.Log("Showing m_Renderer");
                    m_Renderer.enabled = true;
                }

                if (m_LineRenderer != null)
                {
                    //Debug.Log("Showing m_LineRenderer");
                    m_LineRenderer.enabled = true;
                }
            }

            [ClientRpc]
            public virtual void NetShowClientRpc(ClientRpcParams _clientRpcParams)
            {
                NetShow();
            }

            [ServerRpc(RequireOwnership = false)]
            public void RequestSetActiveServerRpc(bool _active)
            {
                isActivated.Value = _active;
            }
            #endregion
        }
    }
}
