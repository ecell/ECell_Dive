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
                                IMlprData,
                                IMlprDataBroadcast,
                                IMlprDataRequest
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
                //StartCoroutine(TryDiveInC());
                if (isReadyForGeneration.Value)
                {
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

            public virtual IEnumerator TryDiveInC()
            {
                yield return null;
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


            #region - IMlprData Methods -
            public virtual void AssembleFragmentedData()
            {
                //isReadyForGeneration = true;
            }

            [ServerRpc(RequireOwnership = false)]
            public void ConfirmSourceDataReceptionServerRpc()
            {
                nbClientReadyLoaded.Value++;
                if (nbClientReadyLoaded.Value == NetworkManager.Singleton.ConnectedClientsIds.Count)
                {
                    isReadyForGeneration.Value = true;
                }
            }

            public void DirectRecieveSourceData(byte[] _sourceDataName, List<byte[]> _fragmentedSourceData)
            {
                fragmentedSourceData = _fragmentedSourceData;
                sourceDataName = _sourceDataName;
                sourceDataNbFrags = _fragmentedSourceData.Count;

                ConfirmSourceDataReceptionServerRpc();
                AssembleFragmentedData();

                BroadcastSourceDataNameServerRpc(sourceDataName);
                BroadcastSourceDataNbFragsServerRpc((ushort)sourceDataNbFrags);
                StartCoroutine(BroadcastSourceDataFragsC(fragmentedSourceData));
            }

            [ServerRpc(RequireOwnership = false)]
            public virtual void RequestSourceDataGenerationServerRpc(ulong _expeditorClientID)
            {
                Debug.LogError("No Generation scheme has been defined for this GameNetModule. " +
                    "Please, override this method and code how you the data stored in this module" +
                    " should be represented in the scene.");
            }
            #endregion

            #region - IMlprDataExchange Methods -
            [ClientRpc]
            public void BroadcastSourceDataFragClientRpc(byte[] _fragment)
            {
                if (IsOwner) return;

                fragmentedSourceData.Add(_fragment);
                if (fragmentedSourceData.Count == sourceDataNbFrags)
                {
                    ConfirmSourceDataReceptionServerRpc();
                    AssembleFragmentedData();
                    //isLoaded = true;
                }
            }

            [ClientRpc]
            public void BroadcastSourceDataNameClientRpc(byte[] _name)
            {
                if (IsOwner) return;

                sourceDataName = _name;
            }

            [ClientRpc]
            public void BroadcastSourceDataNbFragsClientRpc(ushort _sourceDataNbFrags)
            {
                if (IsOwner) return;

                sourceDataNbFrags = _sourceDataNbFrags;
            }

            [ServerRpc]
            public void BroadcastSourceDataFragServerRpc(byte[] _fragment)
            {
                BroadcastSourceDataFragClientRpc(_fragment);
            }

            [ServerRpc]
            public void BroadcastSourceDataNameServerRpc(byte[] _name)
            {
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
                    yield return new WaitForEndOfFrame();
                }
            }
            #endregion
        }
    }
}
