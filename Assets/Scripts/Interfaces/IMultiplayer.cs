using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using ECellDive.Modules;


namespace ECellDive.Interfaces
{
	/// <summary>
	/// The interface defining the logic used for the intermediate object
	/// that will help requesting the server to spawn a root module (e.g.
	/// <see cref="CyJsonModule"/>).
	/// </summary>
	public interface IMlprModuleSpawn
	{
		/// <summary>
		/// List of byte arrays containing the data associated to the module
		/// but fragmented to avoid exceeding the maximum size of a RPC.
		/// </summary>
		public List<byte[]> fragmentedSourceData { get; }

		/// <summary>
		/// The name of the data associated to the module.
		/// </summary>
		byte[] sourceDataName { get; }

		/// <summary>
		/// Transmits the <see cref="fragmentedSourceData"/> to the module
		/// <paramref name="_gameNetModule"/>.
		/// </summary>
		/// <param name="_gameNetModule">
		/// The module to which the data should be transmitted.
		/// </param>
		void GiveDataToModule(GameNetModule _gameNetModule);

		/// <summary>
		/// Sending the reference of the module <paramref name="_networkObjectReference"/>
		/// to the client referenced by <paramref name="_clientRpcParams"/>.
		/// </summary>
		/// <param name="_networkObjectReference">
		/// The reference to the module's NetworkObject.
		/// </param>
		/// <param name="_clientRpcParams">
		/// The client to which the module's reference should be given.
		/// </param>
		[ClientRpc]
		public void GiveNetworkObjectReferenceClientRpc(NetworkObjectReference _networkObjectReference,
														ClientRpcParams _clientRpcParams);

		/// <summary>
		/// Giving the ownership of the module <paramref name="_of"/> to the client
		/// with the ID <paramref name="_newOwnerClientID"/>.
		/// </summary>
		/// <param name="_of">
		/// The module to which the ownership should be given.
		/// </param>
		/// <param name="_newOwnerClientID">
		/// The ID of the client to which the ownership should be given.
		/// </param>
		void GiveOwnership(GameObject _of, ulong _newOwnerClientID);

		/// <summary>
		/// The method calling <see cref="RequestModuleSpawnServerRpc(int, ulong)"/>.
		/// </summary>
		/// <param name="_moduleID">
		/// The ID of the module to spawn. The IDs are hardcoded indexes from the editor in the
		/// <see cref="ECellDive.SceneManagement.DiveScenesManager.modulePrefabs"/>.
		/// </param>
		/// <param name="_dataName">
		/// The name of the data associated to the module.
		/// </param>
		/// <param name="_fragmentedData">
		/// The data associated to the module but fragmented to avoid exceeding the maximum size
		/// of a RPC.
		/// </param>
		public void RequestModuleSpawnFromData(int _moduleID, byte[] _dataName, List<byte[]> _fragmentedData);

		/// <summary>
		/// The client with ID <paramref name="_expeditorClientID"/> requests the server to spawn
		/// a module of type <paramref name="_moduleTypeID"/>.
		/// </summary>
		/// <param name="_moduleTypeID">
		/// The ID of the module to spawn. The IDs are hardcoded indexes from the editor in the
		/// <see cref="ECellDive.SceneManagement.DiveScenesManager.modulePrefabs"/>.
		/// </param>
		/// <param name="_expeditorClientID">
		/// The ID of the client that is requesting the spawn.
		/// </param>
		[ServerRpc]
		public void RequestModuleSpawnServerRpc(int _moduleTypeID, ulong _expeditorClientID);
	}

	/// <summary>
	/// The interface with the base members and methods to associate shareable data
	/// to a module in a multiplayer context.
	/// </summary>
	public interface IMlprData
	{
		/// <summary>
		/// List of byte arrays containing the fragmented data associated to the module.
		/// </summary>
		List<byte[]> fragmentedSourceData { get; }

		/// <summary>
		/// The name of the data associated to the module (in <see
		/// cref="fragmentedSourceData"/>)
		/// </summary>
		byte[] sourceDataName { get; }

		/// <summary>
		/// The number of fragments in <see cref="fragmentedSourceData"/>.
		/// </summary>
		/// <remarks>
		/// It should be equal to <see cref="fragmentedSourceData"/>.Count.
		/// </remarks>
		int sourceDataNbFrags { get; }

		/// <summary>
		/// The number of clients that have received all the fragments from
		/// <see cref="fragmentedSourceData"/>.
		/// </summary>
		NetworkVariable<int> nbClientReadyLoaded { get; }

		/// <summary>
		/// A network variable that is true when all clients have received all
		/// the fragments from <see cref="fragmentedSourceData"/>.
		/// </summary>
		NetworkVariable<bool> isReadyForGeneration { get; }

		/// <summary>
		/// Reassembles the fragments from <see cref="fragmentedSourceData"/> into
		/// a single byte array and interprets it.
		/// </summary>
		/// <remarks>
		/// It is marked as abstract because it MUST be implemented by any divable module.
		/// </remarks>
		abstract void AssembleFragmentedData();

		/// <summary>
		/// Public interface to a coroutine requesting the server to send the name,
		/// the number of fragments and the fragments of the data associated to the module
		/// to all clients.
		/// </summary>
		/// <remarks>
		/// Sequentially calls <see cref="BroadcastSourceDataNameServerRpc(byte[])"/>,
		/// <see cref="BroadcastSourceDataNbFragsServerRpc(ushort)"/> and
		/// <see cref="BroadcastSourceDataFragsC(List{byte[]})"/>.
		/// </remarks>
		IEnumerator BroadcastSourceDataC();

		/// <summary>
		/// The server sends a data fragment to the module to the clients.
		/// </summary>
		/// <param name="_fragment">
		/// The fragment to send.
		/// </param>
		[ClientRpc]
		void BroadcastSourceDataFragClientRpc(byte[] _fragment);

		/// <summary>
		/// The server sends the name of the data associated to the module to the clients.
		/// </summary>
		/// <param name="_name">
		/// The name of the data associated to the module.
		/// </param>
		[ClientRpc]
		void BroadcastSourceDataNameClientRpc(byte[] _name);

		/// <summary>
		/// The server sends the number of fragments to the clients.
		/// </summary>
		/// <param name="_sourceDataNbFrags">
		/// The number of fragments.
		/// </param>
		[ClientRpc]
		void BroadcastSourceDataNbFragsClientRpc(ushort _sourceDataNbFrags);

		/// <summary>
		/// The client who has ownership of the module confirms request the 
		/// server to broadcast the data fragment to the other clients.
		/// </summary>
		/// <param name="_fragment">
		/// The fragment to send.
		/// </param>
		[ServerRpc]
		void BroadcastSourceDataFragServerRpc(byte[] _fragment);

		/// <summary>
		/// The client who has ownership of the module requests the server to
		/// broadcast the name of the data associated to the module to the other clients.
		/// </summary>
		/// <param name="_name">
		/// The name of the data associated to the module.
		/// </param>
		[ServerRpc]
		void BroadcastSourceDataNameServerRpc(byte[] _name);

		/// <summary>
		/// The client who has ownership of the module requests the server to
		/// broadcast the number of fragments to the other clients.
		/// </summary>
		/// <param name="_sourceDataNbFrags">
		/// The number of fragments.
		/// </param>
		[ServerRpc]
		void BroadcastSourceDataNbFragsServerRpc(ushort _sourceDataNbFrags);

		/// <summary>
		/// The coroutine requesting to broadcast all the fragments in
		/// <see cref="fragmentedSourceData"/> to all clients.
		/// </summary>
		/// <param name="_fragmentedSourceData">
		/// The fragments to broadcast.
		/// </param>
		IEnumerator BroadcastSourceDataFragsC(List<byte[]> _fragmentedSourceData);

		/// <summary>
		/// Any client sends a confirmation to the server that it has received all
		/// the fragments from <see cref="fragmentedSourceData"/>.
		/// </summary>
		/// <remarks>
		/// This happens before <see cref="AssembleFragmentedData"/>.
		/// </remarks>
		[ServerRpc(RequireOwnership = false)]
		void ConfirmSourceDataReceptionServerRpc();

		/// <summary>
		/// This is a shortcut to assign values to <see cref="sourceDataName"/>,
		/// and <see cref="fragmentedSourceData"/> for the client who has ownership
		/// of the module.
		/// </summary>
		/// <param name="_sourceDataName">
		/// The name of the data associated to the module.
		/// </param>
		/// <param name="_sourceData">
		/// The fragments of the data associated to the module.
		/// </param>
		void DirectReceiveSourceData(byte[] _sourceDataName, List<byte[]> _sourceData);

		/// <summary>
		/// Requests the server to generate the data associated to the module.
		/// This MUST be implemented by any divable module in order for the data
		/// generation to be broadcasted to the other clients.
		/// </summary>
		/// <param name="_expeditorClientID">
		/// The ID of the client that is requesting the generation.
		/// </param>
		/// <remarks>
		/// RPCs cannot be marked as abstract so we can only define the method as virtual.
		/// Unfortunately, this means that we cannot force the implementation of this method
		/// and that developpers will have to remember to implement it in their modules if it
		/// is divable.
		/// </remarks>
		[ServerRpc(RequireOwnership = false)]
		void RequestSourceDataGenerationServerRpc(ulong _expeditorClientID);

		/// <summary>
		/// Public interface to a coroutine requesting the server to send the name,
		/// the number of fragments and the fragments of the data associated to the module
		/// to the client with the ID <paramref name="_targetClientID"/>.
		/// </summary>
		/// <param name="_targetClientID">
		/// The ID of the client to which the data should be sent.
		/// </param>
		IEnumerator SendSourceDataC(ulong _targetClientID);

		/// <summary>
		/// The server sends a data fragment to the module to the client reprensented
		/// by <paramref name="_clientRpcParams"/>.
		/// </summary>
		/// <param name="_fragment">
		/// The fragment to send.
		/// </param>
		/// <param name="_clientRpcParams">
		/// The struct encoding the information about the client to which the data should be sent.
		/// </param>
		[ClientRpc]
		void SendSourceDataFragClientRpc(byte[] _fragment, ClientRpcParams _clientRpcParams);

		/// <summary>
		/// The server sends the name of the data associated to the module to the client
		/// reprensented by <paramref name="_clientRpcParams"/>.
		/// </summary>
		/// <param name="_name">
		/// The name of the data associated to the module.
		/// </param>
		/// <param name="_clientRpcParams">
		/// The struct encoding the information about the client to which the data should be sent.
		/// </param>
		[ClientRpc]
		void SendSourceDataNameClientRpc(byte[] _name, ClientRpcParams _clientRpcParams);

		/// <summary>
		/// The server sends the number of fragments to the client reprensented
		/// by <paramref name="_clientRpcParams"/>.
		/// </summary>
		/// <param name="_sourceDataNbFrags">
		/// The number of fragments.
		/// </param>
		/// <param name="_clientRpcParams">
		/// The struct encoding the information about the client to which the data should be sent.
		/// </param>
		[ClientRpc]
		void SendSourceDataNbFragsClientRpc(ushort _sourceDataNbFrags, ClientRpcParams _clientRpcParams);

		/// <summary>
		/// The coroutine controlling the sending of all the fragments in
		/// <see cref="fragmentedSourceData"/> to the client reprensented
		/// by <paramref name="_clientRpcParams"/>.
		/// </summary>
		/// <param name="_fragmentedSourceData">
		/// The fragments to send.
		/// </param>
		/// <param name="_clientRpcParams">
		/// The struct encoding the information about the client to which the data should be sent.
		/// </param>
		IEnumerator SendSourceDataFragsC(List<byte[]> _fragmentedSourceData, ClientRpcParams _clientRpcParams);
	}

	/// <summary>
	/// The interface used to manipulate the visibility status of a 
	/// network object. We cannot use Netcode's API for that (see
	/// NetworkObject.NetworkHide(ulong) and NetworkObject.NetworkShow(ulong))
	/// because the Server cannot be affected by it. It is an understandable
	/// behaviour as it seems that this API simply deletes the replicated 
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
		/// client.</param>
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
		/// client.</param>
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