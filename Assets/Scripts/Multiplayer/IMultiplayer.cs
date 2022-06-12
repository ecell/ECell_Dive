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

        void GiveDataToModule(GameNetModule _gameNetModule);

        [ClientRpc]
        public void GiveNetworkObjectReferenceClientRpc(NetworkObjectReference _networkObjectReference,
                                                        ClientRpcParams _clientRpcParams);

        void GiveOwnership(GameObject _of, ulong _newOwnerClientID);

        public void RequestModuleSpawnFromData(int _moduleID, byte[] _dataName, List<byte[]> _fragmentedData);

        [ServerRpc]
        public void RequestModuleSpawnServerRpc(int _moduleTypeID, ulong _expeditorClientID);

    }

    /// <summary>
    /// The interface with the base members and methods to associate shareable data
    /// to a module in a multiplayer context.
    /// </summary>
    public interface IMlprData
    {
        List<byte[]> fragmentedSourceData { get; }

        byte[] sourceDataName { get; }

        int sourceDataNbFrags { get; }

        NetworkVariable<int> nbClientReadyLoaded { get; }

        NetworkVariable<bool> isReadyForGeneration { get; }

        [ServerRpc(RequireOwnership = false)]

        void AssembleFragmentedData();

        void DirectRecieveSourceData(byte[] _sourceDataName, List<byte[]> _sourceData);        

        [ServerRpc(RequireOwnership = false)]
        void ConfirmSourceDataReceptionServerRpc();

        [ServerRpc(RequireOwnership = false)]
        abstract void RequestSourceDataGenerationServerRpc(ulong _expeditorClientID);
    }

    /// <summary>
    /// Interface for a module data owner to distribute the associated data
    /// to every connected client through the server in a multiplayer context.
    /// The implementation will probably require <see cref="IMlprData"/>.
    /// </summary>
    public interface IMlprDataBroadcast
    {
        [ClientRpc]
        void BroadcastSourceDataFragClientRpc(byte[] _fragment);

        [ClientRpc]
        void BroadcastSourceDataNameClientRpc(byte[] _name);
        
        [ServerRpc]
        void BroadcastSourceDataFragServerRpc(byte[] _fragment);

        [ServerRpc]
        void BroadcastSourceDataNameServerRpc(byte[] _name);

        IEnumerator BroadcastSourceDataFragsC(List<byte[]> _fragmentedSourceData);
    }

    /// <summary>
    /// Interface for a client to request to the server the data
    /// associated with a data module in a multiplayer context.
    /// The implementation will probably require <see cref="IMlprData"/>.
    /// </summary>
    public interface IMlprDataRequest
    {

    }
}