using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using ECellDive.Interfaces;
using ECellDive.Modules;
using ECellDive.SceneManagement;
using ECellDive.Utility;

namespace ECellDive.Multiplayer
{
    public class GameNetModuleSpawner : NetworkBehaviour,
                                        IMlprModuleSpawn
    {
        #region - IMlprModuleSpawn Members -
        public List<byte[]> fragmentedSourceData { get; private set; }
        public byte[] sourceDataName { get ; private set; }
        #endregion

        #region - IMlprModuleSpawn Methods -
        public void GiveDataToModule(GameNetModule _gameNetModule)
        {
            LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        "Giving module Data.");
            _gameNetModule.DirectReceiveSourceData(sourceDataName, fragmentedSourceData);
        }

        [ClientRpc]
        public void GiveNetworkObjectReferenceClientRpc(NetworkObjectReference _networkObjectReference,
                                                        ClientRpcParams _clientRpcParams)
        {
            LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        "Receiving ownership of the module that was just spawned.");
            GameObject networkGameObject = _networkObjectReference;
            GiveDataToModule(networkGameObject.GetComponent<GameNetModule>());
        }

        public void GiveOwnership(GameObject _of, ulong _newOwnerClientID)
        {
            _of.GetComponent<NetworkObject>().ChangeOwnership(_newOwnerClientID);
        }

        public void RequestModuleSpawnFromData(int _moduleTypeID, byte[] _dataName, List<byte[]> _fragmentedData)
        {
            fragmentedSourceData = _fragmentedData;
            sourceDataName = _dataName;
            RequestModuleSpawnServerRpc(_moduleTypeID, NetworkManager.Singleton.LocalClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestModuleSpawnServerRpc(int _moduleTypeID, ulong _expeditorClientID)
        {
            LogSystem.refLogManager.AddMessage(LogSystem.MessageTypes.Debug,
                        "Server Received a request for spawn.");

            GameObject player = NetworkManager.Singleton.ConnectedClients[_expeditorClientID].PlayerObject.gameObject;
            Vector3 pos = Positioning.PlaceInFrontOfTarget(player.GetComponentInChildren<Camera>().transform, 2f, 0.8f);

            GameObject module = DiveScenesManager.Instance.SpawnModuleInScene(
                GameNetPortal.Instance.netSessionPlayersDataMap[_expeditorClientID].currentScene,
                _moduleTypeID,
                pos);

            GameNetPortal.Instance.dataModules.Add(module.GetComponent<IMlprData>());
            
            //Giving ownership to the client who initially made 
            //the spawning request
            GiveOwnership(module, _expeditorClientID);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { _expeditorClientID }
                }
            };

            //Sending the spawned object reference back to the client
            //which initially made the request so that he can continue his 
            //process.
            GiveNetworkObjectReferenceClientRpc(module, clientRpcParams);
        }
        #endregion
    }
}

