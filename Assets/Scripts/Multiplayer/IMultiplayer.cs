using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using ECellDive.Modules;


namespace ECellDive.Interfaces
{
    public interface IMlprModuleSpawn
    {
        public List<byte[]> fragmentedSourceData { get; }

        byte[] sourceDataName { get; }

        GameNetModule lastSpawnedModule { get; }

        void GiveDataToModule();

        [ClientRpc]
        public void GiveNetworkObjectReferenceClientRpc(NetworkObjectReference _networkObjectReference,
                                                        ClientRpcParams _clientRpcParams);

        void GiveOwnership(GameObject _of, ulong _newOwnerClientID);

        public void RequestModuleSpawnFromData(int _moduleID, byte[] _dataName, List<byte[]> _fragmentedData);

        [ServerRpc]
        public void RequestModuleSpawnServerRpc(int _moduleTypeID, ulong _expeditorClientID);

    }

    public interface IMlprDataExchange
    {
        List<byte[]> fragmentedSourceData { get; }

        byte[] sourceDataName { get; }

        NetworkVariable<bool> isReadyForAssembling { get; }

        bool isLoaded { get; }

        bool isReadyForGeneration { get; }

        void AssembleFragmentedData();

        void DirectRecieveSourceData(byte[] _sourceDataName, List<byte[]> _sourceData);

        [ClientRpc]
        void ForwardAuthorizationToAssembleClientRpc(ClientRpcParams _clientRpcParams);

        //[ClientRpc]
        //void ForwardSourceDataFragClientRpc(ushort _fragmentIdx,
        //                                    byte[] _fragment,
        //                                    ClientRpcParams _clientRpcParams);
        [ClientRpc]
        void ForwardSourceDataFragClientRpc(byte[] _fragment,
                                            ClientRpcParams _clientRpcParams);

        [ClientRpc]
        void ForwardSourceDataNameClientRpc(byte[] _name,
                                            ClientRpcParams _clientRpcParams);

        //[ClientRpc]
        //void ForwardSourceDataNbFragsClientRpc(ushort _nbFragments,
        //                                       ClientRpcParams _clientRpcParams);

        [ClientRpc]
        void RequestSourceDataClientRpc(ulong _expeditorClientID, ClientRpcParams _clientRpcParams);

        [ServerRpc(RequireOwnership = false)]        
        void RequestSourceDataServerRpc(ulong _expeditorClientID, ulong _dataOwnerCliendID);

        [ServerRpc]
        void SendAuthorizationToAssembleServerRpc(ulong _recipientClienID);

        //[ServerRpc]
        //void SendSourceDataFragServerRpc(ushort _fragmentIdx, byte[] _fragment, ulong _recipientClienID);
        
        [ServerRpc]
        void SendSourceDataFragServerRpc(byte[] _fragment, ulong _recipientClienID);

        [ServerRpc]
        void SendSourceDataNameServerRpc(byte[] _name, ulong _recipientClienID);

        //[ServerRpc]
        //void SendSourceDataNbFragsServerRpc(ushort _nbFragments, ulong _recipientClienID);

        IEnumerator SendSourceDataFragsC(ulong _recipientClienID);
    }
}