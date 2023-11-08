using System.Collections;
using Unity.Netcode;
using UnityEngine.InputSystem;


namespace ECellDive.Interfaces
{
    /// <summary>
    /// The interface to describe the required elements to implement a module
    /// that a user can dive in.
    /// </summary>
    public interface IDive
    {
        /// <summary>
        /// Boolean to indicate that a player is currently diving in the module.
        /// </summary>
        bool isDiving { get; }

        /// <summary>
        /// A variable synchronized over the multiplayer network to
        /// inform the clients when a module has finished generating
        /// its content and is therefore ready to accept divers.
        /// </summary>
        NetworkVariable<bool> isReadyForDive { get; }

        /// <summary>
        /// The id of the dive scene where the module was added.
        /// </summary>
        NetworkVariable<int> rootSceneId { get; set; }

        /// <summary>
        /// The id of the dive scene the module will transfer
        /// the divers to.
        /// </summary>
        NetworkVariable<int> targetSceneId { get; set; }

        /// <summary>
        /// The public interface to call when a user is trying to dive into
        /// a module that another user has already generated: all the
        /// physical objects representing the data has ALREADY been instantiated
        /// in the scene and the NetworkObjects have ALREADY been replicated
        /// across the multiplayer network.
        /// </summary>
        /// <remarks>
        /// Asynchronous: it calls the coroutine <see cref="DirectDiveInC"/>
        /// </remarks>
        void DirectDiveIn();

        /// <summary>
        /// Coroutine started by <see cref="DirectDiveIn"/>.
        /// Transfers the user to the dive scene associated to the module.
        /// </summary>
        IEnumerator DirectDiveInC();

        /// <summary>
        /// The public interface to call when a user is trying to dive into
        /// a module that has NOT been generated yet: all the physical objects
        /// representing the data has NOT been instantiated yet in the
        /// scene and the NetworkObjects have NOT been replicated yet
        /// across the multiplayer network.
        /// </summary>
        /// <remarks>
        /// Asynchronous: it calls the coroutine <see cref="GenerativeDiveInC"/>
        /// </remarks>
        void GenerativeDiveIn();

        /// <summary>
        /// Coroutine started by <see cref="GenerativeDiveIn"/>.
        /// Generates the content of the module before transfering the diver
        /// to the dive scene.
        /// </summary>
        abstract IEnumerator GenerativeDiveInC();

        /// <summary>
        /// The public interface to call when a user wants to dive into a 
        /// module.
        /// </summary>
        void TryDiveIn();
    }
}
