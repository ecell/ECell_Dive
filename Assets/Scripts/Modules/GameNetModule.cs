using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using ECellDive.Utility;
using ECellDive.UI;
using ECellDive.Interfaces;

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
                                IMlprDataExchange
        {
            [Header("Module Info")]
            public TextMeshProUGUI refName;

            ClientRpcParams cachedClientRpcParams;

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

            //[SerializeField] private bool m_finalLayer = false;
            //public bool isFinalLayer
            //{
            //    get => m_finalLayer;
            //    set => m_finalLayer = value;
            //}
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

            [SerializeField] private Color m_defaultColor;
            public Color defaultColor
            {
                get => m_defaultColor;
                set => SetDefaultColor(value);
            }

            [SerializeField] private Color m_highlightColor;
            public Color highlightColor
            {
                get => m_highlightColor;
                set => SetHighlightColor(value);
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

            #region - IMlprDataExchange -
            public byte[] sourceDataName
            {
                get;
                protected set;
            }

            private List<byte[]> m_fragmentedSourceData = new List<byte[]>();
            public List<byte[]> fragmentedSourceData
            {
                get => m_fragmentedSourceData;
                protected set => m_fragmentedSourceData = value;
            }

            private NetworkVariable<bool> m_isReadyForAssembling = new NetworkVariable<bool>(false,
                NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
            public NetworkVariable<bool> isReadyForAssembling
            {
                get => m_isReadyForAssembling;
                protected set { m_isReadyForAssembling = value; }
            }

            bool m_isLoaded = false;
            public bool isLoaded
            {
                get => m_isLoaded;
                protected set { m_isLoaded = value; }
            }

            bool m_isReadyForGeneration = false;
            public bool isReadyForGeneration
            {
                get => m_isReadyForGeneration;
                protected set { m_isReadyForGeneration = value; }
            }

            #endregion

            protected virtual void Awake()
            {
                areVisible = false;

                diveActions.leftController.action.performed += TryDiveIn;
                diveActions.rightController.action.performed += TryDiveIn;

                m_displayInfoTagsActions.leftController.action.performed += ManageInfoTagsDisplay;
                m_displayInfoTagsActions.rightController.action.performed += ManageInfoTagsDisplay;
            }

            public override void OnDestroy()
            {
                diveActions.leftController.action.performed -= TryDiveIn;
                diveActions.rightController.action.performed -= TryDiveIn;

                m_displayInfoTagsActions.leftController.action.performed -= ManageInfoTagsDisplay;
                m_displayInfoTagsActions.rightController.action.performed -= ManageInfoTagsDisplay;
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

            public void SetName(string _name)
            {
                refName.text = _name;
            }

            /// <summary>
            /// Makes sure the name of the module's name faces the
            /// Player's POV and is therefore readable.
            /// </summary>
            public void ShowNameToPlayer()
            {
                Positioning.UIFaceTarget(refName.gameObject.transform.parent.gameObject, Camera.main.transform);
            }

            #region - IDive Methods -

            [ServerRpc(RequireOwnership = false)]
            public void BroadcastIsReadyForDiveServerRpc()
            {
                isReadyForDive.Value = true;
            }

            public void DirectDiveIn()
            {
                StartCoroutine(DirectDiveInC());
            }

            public virtual IEnumerator DirectDiveInC()
            {
                Debug.LogError($"Direct dive in {gameObject.name}:{refName.text} but no" +
                    $"custom behaviour has been defined for that type of module");
                yield return null;
            }

            public void GenerativeDiveIn()
            {
                StartCoroutine(GenerativeDiveInC());
            }

            public virtual IEnumerator GenerativeDiveInC()
            {
                Debug.LogError($"Generative dive in {gameObject.name}:{refName.text} but no" +
                    $"custom behaviour has been defined for that type of module");
                yield return null;
            }

            public void TryDiveIn(InputAction.CallbackContext _ctx)
            {
                StartCoroutine(TryDiveInC());
            }

            public virtual IEnumerator TryDiveInC()
            {
                if (isReadyForAssembling.Value)//data has been copied in the owner version of the netobject
                {
                    if (!isReadyForGeneration)//need to assemble data
                    {
                        if (!IsOwner)//equivalent to test "isLoaded"
                        {
                            RequestSourceDataServerRpc(NetworkManager.Singleton.LocalClientId,
                                                        OwnerClientId);

                            yield return new WaitUntil(() => isLoaded);

                            AssembleFragmentedData();
                        }
                    }

                    if (isReadyForDive.Value)
                    {
                        DirectDiveIn();//make visible the network
                    }
                    else
                    {
                        GenerativeDiveIn();//generate and restrict visibility of network
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

            public virtual void SetDefaultColor(Color _c)
            {
                m_defaultColor = _c;
            }

            public virtual void SetHighlightColor(Color _c)
            {
                m_highlightColor = _c;
            }

            public virtual void SetHighlight()
            {
            }

            public virtual void UnsetHighlight()
            {
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
                    _infoTag.GetComponent<InfoDisplayManager>().ShowInfoToPlayer();
                }
            }
            #endregion

            #region - IMlprDataExchange -

            public virtual void AssembleFragmentedData()
            {
                isReadyForGeneration = true;
            }

            public void DirectRecieveSourceData(byte[] _sourceDataName, List<byte[]> _fragmentedSourceData)
            {
                sourceDataName = _sourceDataName;
                fragmentedSourceData = _fragmentedSourceData;
                isLoaded = true;
                isReadyForAssembling.Value = true;

                AssembleFragmentedData();
            }

            [ClientRpc]
            public virtual void ForwardAuthorizationToAssembleClientRpc(ClientRpcParams _clientRpcParams)
            {
                isLoaded = true;
            }

            [ClientRpc]
            public void ForwardSourceDataFragClientRpc(byte[] _fragment,
                                                        ClientRpcParams _clientRpcParams)
            {
                fragmentedSourceData.Add(_fragment);
            }

            [ClientRpc]
            public void ForwardSourceDataNameClientRpc(byte[] _name,
                                                        ClientRpcParams _clientRpcParams)
            {
                sourceDataName = _name;
            }

            [ClientRpc]
            public void RequestSourceDataClientRpc(ulong _expeditorClientID, ClientRpcParams _clientRpcParams)
            {
                SendSourceDataNameServerRpc(sourceDataName, _expeditorClientID);
                StartCoroutine(SendSourceDataFragsC(_expeditorClientID));
            }

            [ServerRpc(RequireOwnership = false)]
            public void RequestSourceDataServerRpc(ulong _expeditorClientID, ulong _dataOwnerCliendID)
            {

                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[1] { _dataOwnerCliendID }
                    }
                };
                RequestSourceDataClientRpc(_expeditorClientID, clientRpcParams);
            }

            [ServerRpc]
            public void SendAuthorizationToAssembleServerRpc(ulong _recipientClienID)
            {
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[1] { _recipientClienID }
                    }
                };
                ForwardAuthorizationToAssembleClientRpc(clientRpcParams);
            }

            [ServerRpc]
            public void SendSourceDataFragServerRpc(byte[] _fragment, ulong _recipientClienID)
            {
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[1] { _recipientClienID }
                    }
                };
                ForwardSourceDataFragClientRpc(_fragment,
                                                clientRpcParams);
            }

            [ServerRpc]
            public void SendSourceDataNameServerRpc(byte[] _name, ulong _recipientClienID)
            {
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[1] { _recipientClienID }
                    }
                };
                ForwardSourceDataNameClientRpc(_name, clientRpcParams);
            }

            public IEnumerator SendSourceDataFragsC(ulong _recipientClienID)
            {
                
                foreach (byte[] _frag in fragmentedSourceData)
                {
                    yield return new WaitForEndOfFrame();
                }
                SendAuthorizationToAssembleServerRpc(_recipientClienID);
            }

            #endregion
        }
    }
}
