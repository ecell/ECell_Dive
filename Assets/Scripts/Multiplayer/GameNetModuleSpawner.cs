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
        public GameNetModule lastSpawnedModule { get; private set; }
        #endregion

        #region - IMlprModuleSpawn Methods -
        public void GiveDataToModule()
        {
            lastSpawnedModule.DirectRecieveSourceData(sourceDataName, fragmentedSourceData);
        }

        [ClientRpc]
        public void GiveNetworkObjectReferenceClientRpc(NetworkObjectReference _networkObjectReference,
                                                        ClientRpcParams _clientRpcParams)
        {
            GameObject networkGameObject = _networkObjectReference;
            lastSpawnedModule = networkGameObject.GetComponent<GameNetModule>();
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

        [ServerRpc]
        public void RequestModuleSpawnServerRpc(int _moduleTypeID, ulong _expeditorClientID)
        {
            ModuleData cyJsonMD = new ModuleData
            {
                typeID = 4 // 4 is the type ID of a CyJsonModule
            };
            ModulesData.AddModule(cyJsonMD);
            Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 2f, 0.8f);
            GameObject cyJsonModule = ScenesData.refSceneManagerMonoBehaviour.InstantiateGOOfModuleData(cyJsonMD, pos);

            //Synchroinising across the network
            cyJsonModule.GetComponent<NetworkObject>().Spawn();
            
            //Giving ownership to the client who initially made 
            //the spawning request
            GiveOwnership(cyJsonModule, _expeditorClientID);

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
            GiveNetworkObjectReferenceClientRpc(cyJsonModule, clientRpcParams);

            GiveDataToModule();
        }
        #endregion
    }
}

