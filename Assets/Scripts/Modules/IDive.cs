using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


namespace ECellDive.Interfaces
{
    public interface IDive
    {
        ControllersSymetricAction diveActions { get; set; }

        //TODO: refactor modules to separate modules that are shared on the
        //multiplayer network from modules that are only tools to manipulate
        //data for the player. When refactoring, isFinalLayer, will not be
        //needed anymore
        //bool isFinalLayer { get; set; }

        NetworkVariable<bool> isReadyForDive { get; }

        /// <summary>
        /// Used by the client that is generating the content of a module
        /// (i.e. instantiating all the gameObjects/networkObjects that
        /// physically represents the data stored by a module) to ask the server
        /// to update the networkVariable <see cref="isReadyForDive"/>.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        void BroadcastIsReadyForDiveServerRpc();

        /// <summary>
        /// The public interface to call when a user is trying to dive into
        /// a module that another user has already generated: all the
        /// physical objects representing the data has ALREADY been instantiated
        /// in the scene and the NetworkObjects have ALREADY been replicated
        /// across the multiplayer network.
        /// </summary>
        void DirectDiveIn();

        /// <summary>
        /// Coroutine started by <see cref="DirectDiveIn"/>.
        /// </summary>
        abstract IEnumerator DirectDiveInC();

        /// <summary>
        /// The public interface to call when a user is trying to dive into
        /// a module that has NOT been generated yet: all the physical objects
        /// representing the data has NOT been instantiated yet in the
        /// scene and the NetworkObjects have NOT been replicated yet
        /// across the multiplayer network.
        /// </summary>
        void GenerativeDiveIn();

        /// <summary>
        /// Coroutine started by <see cref="GenerativeDiveIn"/>.
        /// </summary>
        abstract IEnumerator GenerativeDiveInC();

        /// <summary>
        /// The public interface to call when a user wants to dive into a 
        /// module. Performs tests to check whether the data of the module is
        /// accessible and whether a user has already generated the physical
        /// represenbtation of the data.
        /// </summary>
        /// <param name="_ctx">Input action callback</param>
        void TryDiveIn(InputAction.CallbackContext _ctx);

        /// <summary>
        /// Coroutine started by <see cref="TryDiveIn(InputAction.CallbackContext)"/>.
        /// </summary>
        abstract IEnumerator TryDiveInC();
    }
}
