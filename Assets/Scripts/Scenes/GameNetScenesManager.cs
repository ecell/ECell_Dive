using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using ECellDive.Modules;
using ECellDive.Multiplayer;
using ECellDive.PlayerComponents;

namespace ECellDive
{
    namespace SceneManagement
    {
        [System.Serializable]
        public struct DivingAnimationData
        {
            [Tooltip("The gameobject with the diving animator")]
            public Animator refAnimator;

            [Tooltip("The minimum time we wait for the dive.")]
            [Min(1f)] public float duration;
        }

        public struct SceneData
        {
            public int sceneID;
            public int parentSceneID;

            /// <summary>
            /// List of NetworkManager Client Ids that are present in the scene
            /// </summary>
            public List<ulong> inDivers;

            /// <summary>
            /// List of NetworkManager Client Ids that are NOT present in the scene
            /// </summary>
            public List<ulong> outDivers;

            /// <summary>
            /// List of Network Object. Some of them are potential seeds for child scenes.
            /// </summary>
            public List<GameNetModule> loadedModules;

            /// <summary>
            /// Indeces of sceneData in the sceneBank of Dive scenes that take root in this scene.
            /// </summary>
            //public NetworkList<int> childrenScenes; 

            public SceneData(int _sceneID, int _parentSceneID)
            {
                sceneID = _sceneID;
                parentSceneID = _parentSceneID;
                inDivers = new List<ulong>();
                outDivers = new List<ulong>();
                loadedModules = new List<GameNetModule>();
                //childrenScenes = new NetworkList<int>();
            }

            //public void AddChildScene(int _childSceneIdx)
            //{
            //    childrenScenes.Add(_childSceneIdx);
            //}

            public void AddOutDiver(ulong _diverClientId)
            {
                outDivers.Add(_diverClientId);
            }

            public void AddModule(GameNetModule _gameNetModule)
            {
                loadedModules.Add(_gameNetModule);
            }

            public void DiverGetsIn(ulong _diverClientId)
            {
                outDivers.Remove(_diverClientId);
                inDivers.Add(_diverClientId);
            }

             public void DiverGetsOut(ulong _diverClientId)
            {
                inDivers.Remove(_diverClientId);
                outDivers.Add(_diverClientId);
            }            

            /// <summary>
            /// For Debug.
            /// </summary>
            public override string ToString()
            {
                string inDiversStr = "";
                foreach(ulong id in inDivers)
                {
                    inDiversStr += id.ToString()+" ";
                }
                inDiversStr += "\n";

                string outDiversStr = "";
                foreach(ulong id in outDivers)
                {
                    outDiversStr += id.ToString()+" ";
                }
                outDiversStr += "\n";

                string loadedModulesStr = "";
                foreach (GameNetModule _gameNetMod in loadedModules)
                {
                    loadedModulesStr += _gameNetMod.name.ToString() + " ";
                }
                loadedModulesStr += "\n";

                //string childrenScenesIdsStr = "";
                //foreach (ulong id in childrenScenes)
                //{
                //    childrenScenesIdsStr += id.ToString() + " ";
                //}
                //childrenScenesIdsStr += "\n";

                string final = $"Scene Id: {sceneID}\n" +
                               $"Parent Scene Id: {parentSceneID}\n" +
                               $"In Divers: " + inDiversStr +
                               $"Out Divers: " + outDiversStr +
                               $"Loaded modules: " + loadedModulesStr;// +
                               //$"Clidren scenes: " + childrenScenesIdsStr;

                return final;
            }
        }

        public class GameNetScenesManager : NetworkBehaviour
        {
            public static GameNetScenesManager Instance { get; private set; }

            /// <summary>
            /// A list of prefabs that may be instantiated by the user at runtime.
            /// Every prefab MUST have a <see cref="GameNetModule"/> and <see cref="NetworkObject"/>
            /// component. We advise to use this list for root data modules (e.g. CyJsonModule) only.
            /// "Submodules" that may be instantiated as a result of a module's data generation
            /// should be referenced in the script detailing the said data generation process.
            /// </summary>
            public GameObject[] modulePrefabs;
            public DivingAnimationData divingAnimationData;

            public static List<SceneData> scenesBank = new List<SceneData>();

            public override void OnNetworkSpawn()
            {
                Instance = this;
                if (IsServer)
                {
                    Debug.Log("Spawning Scene Management");
                    int startSceneId = AddNewDiveScene(-1);
                    DiverGetsInServerRpc(0, NetworkManager.Singleton.LocalClientId);
                    //NetworkManager.Singleton.OnClientConnectedCallback += clientId => DiverGetsInServerRpc(0, clientId);
                    //NetworkManager.Singleton.OnClientConnectedCallback += e => DebugScene();
                }
            }

            public override void OnNetworkDespawn()
            {
                if (IsServer)
                {
                    //NetworkManager.Singleton.OnClientConnectedCallback -= clientId => DiverGetsInServerRpc(0, clientId);
                    //NetworkManager.Singleton.OnClientConnectedCallback -= e => DebugScene();
                }
            }

            [ServerRpc]
            public void DiverGetsInServerRpc(int _sceneId, ulong _diverClientId)
            {
                Debug.Log($"Diver {_diverClientId} is entering scene {_sceneId}");
                scenesBank[_sceneId].DiverGetsIn(_diverClientId);
            }

            [ServerRpc]
            public void DiverGetsOutServerRpc(int _sceneId, ulong _diverClientId)
            {
                Debug.Log($"Diver {_diverClientId} is leaving scene {_sceneId}");
                scenesBank[_sceneId].DiverGetsOut(_diverClientId);
            }

            /// <summary>
            /// Called to instantiate a new scene upon diving in a module
            /// </summary>
            public int AddNewDiveScene(int _parentSceneId)
            {
                SceneData newScene = new SceneData(scenesBank.Count, _parentSceneId);
                foreach (ulong _clientId in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    newScene.AddOutDiver(_clientId);
                }
                scenesBank.Add(newScene);
                return newScene.sceneID;
            }

            public void DebugScene()
            {
                foreach (SceneData sceneData in scenesBank)
                {
                    Debug.Log(sceneData);
                }
            }

            /// <summary>
            /// Calls <see cref="GameNetModule.NetHide"/> for every <see cref=
            /// "GameNetModule"/> of the scene with <paramref name="_sceneID"/>
            /// for the local client.
            /// </summary>
            /// <param name="_sceneID">Index of the scene in <see cref="scenesBank"/></param>
            //[ServerRpc(RequireOwnership = false)]
            private IEnumerator HideScene(int _sceneID, ulong _outDiverClientId)
            {
                //Updating divers for the scene in the Scene bank
                scenesBank[_sceneID].DiverGetsOut(_outDiverClientId);

                Debug.Log($"Hiding scene {_sceneID} for for client {_outDiverClientId}");
                DebugScene();

                ClientRpcParams outDiverClientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { _outDiverClientId }
                    }
                };

                int moduleCounter = 0;
                //Hide every module of the scene to the diver that is leaving
                foreach (GameNetModule _gameNetMod in scenesBank[_sceneID].loadedModules)
                {
                    _gameNetMod.NetHideClientRpc(outDiverClientRpcParams);
                    moduleCounter++;

                    if (moduleCounter == 50)
                    {
                        //Debug.Log("Waiting for end of Frame");
                        yield return new WaitForEndOfFrame();
                        moduleCounter = 0;
                    }
                }

                ClientRpcParams inDiversRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = scenesBank[_sceneID].inDivers.ToArray()
                    }
                };

                GameObject diverGo = NetworkManager.Singleton.ConnectedClients[_outDiverClientId].PlayerObject.gameObject;
                //Hide the out diver from all the in-divers
                diverGo.GetComponent<Player>().NetHideClientRpc(inDiversRpcParams);

                //Hide all the in-divers from the out-diver
                foreach (ulong _inDiverCliendId in scenesBank[_sceneID].inDivers)
                {
                    Debug.Log($"Hiding {_inDiverCliendId} to {_outDiverClientId}");
                    diverGo = NetworkManager.Singleton.ConnectedClients[_inDiverCliendId].PlayerObject.gameObject;
                    diverGo.GetComponent<Player>().NetHideClientRpc(outDiverClientRpcParams);
                }
            }

            /// <summary>
            /// Calls <see cref="GameNetModule.NetShow"/> for every <see cref=
            /// "GameNetModule"/> of the scene with <paramref name="_sceneID"/>
            /// for the local client.
            /// </summary>
            /// <param name="_sceneID">Index of the scene in <see cref="scenesBank"/></param>
            //[ServerRpc(RequireOwnership = false)]
            private IEnumerator ShowScene(int _sceneID, ulong _newInDiverClientId)
            {
                //Debug.Log($"Showing scene {_sceneID} for for client {_newInDiverClientId}");
                //DebugScene();
                ClientRpcParams newInDiverClientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { _newInDiverClientId }
                    }
                };

                int moduleCounter = 0;
                //Show every modules of the new to the new diver
                foreach (GameNetModule _gameNetMod in scenesBank[_sceneID].loadedModules)
                {
                    _gameNetMod.NetShowClientRpc(newInDiverClientRpcParams);
                    moduleCounter++;

                    if(moduleCounter == 50)
                    {
                        yield return new WaitForEndOfFrame();
                        moduleCounter = 0;
                    }
                }

                ClientRpcParams oldInDiversRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = scenesBank[_sceneID].inDivers.ToArray()
                    }
                };

                GameObject diverGo = NetworkManager.Singleton.ConnectedClients[_newInDiverClientId].PlayerObject.gameObject;

                //Show the the new diver to all the already present in-Divers
                diverGo.GetComponent<Player>().NetShowClientRpc(oldInDiversRpcParams);

                //Show all already present in-Divers to the new in Diver
                foreach (ulong _oldInDiverCliendId in scenesBank[_sceneID].inDivers)
                {
                    Debug.Log($"Showing {_oldInDiverCliendId} to {_newInDiverClientId}");
                    diverGo = NetworkManager.Singleton.ConnectedClients[_oldInDiverCliendId].PlayerObject.gameObject;
                    diverGo.GetComponent<Player>().NetShowClientRpc(newInDiverClientRpcParams);
                }

                //Updating the scene's data in the scene bank once we finished showing everyone
                scenesBank[_sceneID].DiverGetsIn(_newInDiverClientId);
            }

            /// <summary>
            /// Instantiates a game object and spawns it for replication across the network.
            /// This method can only be called on the SERVER. So, be sure to call from a server object
            /// or called inside a ServerRpc.
            /// </summary>
            /// <param name="_sceneId">The Id of the scene in which it the new game object is instantiated.</param>
            /// <param name="_prefab">The gameobject to instantiate and spawn.
            /// It MUST be a <see cref="GameNetModule"/> and a <see cref="NetworkObject"/>.</param>
            /// <param name="_position">The position at which to instantiate the gameobject.</param>
            /// <returns>The game object that got instantiated & spawned.</returns>
            public GameObject SpawnModuleInScene(int _sceneId, GameObject _prefab, Vector3 _position)
            {
                GameObject go = Instantiate(_prefab, _position, Quaternion.identity);

                GameNetModule gameNetModule = go.GetComponent<GameNetModule>();
                gameNetModule.rootSceneId.Value = _sceneId;

                go.GetComponent<NetworkObject>().Spawn();
                scenesBank[_sceneId].AddModule(gameNetModule);

                return go;
            }

            /// <summary>
            /// Instantiates a game object and spawns it for replication across the network.
            /// This method can only be called on the SERVER. So, be sure to call from a server object
            /// or called inside a ServerRpc.
            /// </summary>
            /// <param name="_sceneId">The Id of the scene in which it the new game object is instantiated.</param>
            /// <param name="_prefabIdx">The index of the game object in the .
            /// It MUST be a <see cref="GameNetModule"/> and a <see cref="NetworkObject"/>.</param>
            /// <param name="_position">The position at which to instantiate the gameobject.</param>
            /// <param name="hide">Whether to call <see cref="GameNetModule.NetHide"/> BEFORE spawn
            /// to globally hide the network object.</param>
            /// <returns>The game object that got instantiated & spawned.</returns>
            public GameObject SpawnModuleInScene(int _sceneId, int _prefabIdx, Vector3 _position)
            {
                GameObject go = Instantiate(modulePrefabs[_prefabIdx], _position, Quaternion.identity);

                GameNetModule gameNetModule = go.GetComponent<GameNetModule>();
                gameNetModule.rootSceneId.Value = _sceneId;

                go.GetComponent<NetworkObject>().Spawn();
                scenesBank[_sceneId].AddModule(gameNetModule);

                return go;
            }

            [ServerRpc(RequireOwnership = false)]
            public void SwitchingScenesServerRpc(int _from, int _to, ulong _clientId)
            {
                StartCoroutine(HideScene(_from, _clientId));

                StartCoroutine(ShowScene(_to, _clientId));

                GameNetPortal.Instance.netSessionPlayersDataMap[_clientId].SetSceneId(_to);
            }

            public void Resurface()
            {
                StartCoroutine(ResurfaceC());
            }

            private IEnumerator ResurfaceC()
            {
                //divingData.refAnimator.SetTrigger("DiveStart");

                yield return new WaitForSeconds(divingAnimationData.duration);

                //divingData.refAnimator.SetTrigger("DiveEnd");
            }
        }
    }
}

