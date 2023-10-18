using System.Collections;
using Unity.Netcode;

namespace ECellDive.Modules
{
    /// <summary>
    /// A dummy game net module.
    /// This only serves the purpose of demonstrating how to create a game net module
    /// and which methods must be implemented.
    /// It does not do anything. It is only attached to the game object called 
    /// "BaseGameNetModule" for demonstration. DO NOT REUSE THIS CLASS.
    /// </summary>
    public class DummyGameNetModule : GameNetModule
    {
        #region - GameNetModule IDive Methods -
        /// <inheritdoc/>
        public override IEnumerator GenerativeDiveInC()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region - GameNetModule IMlprData Methods -
        /// <inheritdoc/>
        public override void AssembleFragmentedData()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        [ServerRpc]
        public override void RequestSourceDataGenerationServerRpc(ulong _expeditorClientID)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
