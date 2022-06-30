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


    /// <summary>
    /// The interface used to manipulate the visibility status of a 
    /// network object. We cannot use Netcode's API for that (<see
    /// cref="NetworkObject.NetworkHide(ulong)"/> and <see cref=
    /// "NetworkObject.NetworkShow(ulong)"/>) because the Server
    /// cannot be affected by it. It is an understandable behaviour 
    /// as it seems that this API  simply deletes the replicated 
    /// version of a NetworkObject for a target client.
    /// However, we also want to the host to not see some NetworkObject.
    /// Namely, the content of a scene that the host may not have dived
    /// in.
    /// <para/>
    /// So, we define our own NetworkObject visibility scheme
    /// which will unable network-replicated but locally executed
    /// activation/deactivate of the whole gameobject when there are no
    /// divers that are interacting with them; and hide/show the graphics
    /// when at least one diver is currently seeing them but not everyone
    /// should. That way, an object can be activate but hidden: it will then
    /// receive network updates related to another client interacting with it
    /// but will stay hidden from the local client.
    /// </summary>
    public interface IMlprVisibility
    {
        /// <summary>
        /// Network variable keeping track of whether a NetworkObject's
        /// gameobject is selfActive = true for at least one replicated
        /// version of it on the network.
        /// </summary>
        NetworkVariable<bool> isActivated { get; }
        //NetworkVariable<bool> isVisible { get; }

        /// <summary>
        /// Intended to active/deactivate the gameobject's version 
        /// of the local client on value change of <see cref="isActivated"/>
        /// </summary>
        abstract void ManageActivationStatus(bool _previous, bool _current);

        /// <summary>
        /// Intended to disable graphics (and Colliders or anything
        /// else that could allow the local client to interact with
        /// the game object) for the gameobject's version of the
        /// local client.
        /// </summary>
        abstract void NetHide();
        
        /// <summary>
        /// The equivalent of <see cref="NetHide"/> but for the server
        /// to call.
        /// </summary>
        /// <param name="_clientRpcParams">Params to target a specific
        /// client.<seealso cref="ClientRpcParams"/></param>
        [ClientRpc]
        abstract void NetHideClientRpc(ClientRpcParams _clientRpcParams);

        /// <summary>
        /// Intended to enable graphics (and Colliders or anything
        /// else that could allow the local client to interact with
        /// the game object) for the gameobject's version of the
        /// local client.
        /// </summary>
        [ClientRpc]
        abstract void NetShow();

        /// <summary>
        /// The equivalent of <see cref="NetShow"/> but for the server
        /// to call.
        /// </summary>
        /// <param name="_clientRpcParams">Params to target a specific
        /// client.<seealso cref="ClientRpcParams"/></param>
        [ClientRpc]
        abstract void NetShowClientRpc(ClientRpcParams _clientRpcParams);

        /// <summary>
        /// A client requests the server to change the value of
        /// <see cref="isActivated"/>;
        /// </summary>
        /// <param name="_active"></param>
        [ServerRpc(RequireOwnership = false)]
        void RequestSetActiveServerRpc(bool _active);
        
    }
}