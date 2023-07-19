using UnityEngine;
using Unity.Netcode;


namespace ECellDive.Multiplayer
{
    /// <summary>
    /// Makes sure that position and rotation of the network objects are synchronized
    /// among all replicates on every client.
    /// </summary>
    /// <remarks> This serves the same purpose as the built-in NetworkTransform of Unity
    /// Netcode for GameObjects except that it makes the update on the client side
    /// instead of the server's. This can maybe be replaced by Networktransform but we couldn't
    /// achieved the desired results for now.</remarks>
    public class NetAvatarTransformTracker : NetworkBehaviour
    {
        private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(
                                    default,
                                    NetworkVariableBase.DefaultReadPerm,
                                    NetworkVariableWritePermission.Owner);

        private NetworkVariable<Quaternion> rotation = new NetworkVariable<Quaternion>(
                                    default,
                                    NetworkVariableBase.DefaultReadPerm,
                                    NetworkVariableWritePermission.Owner);

        private void Update()
        {
            if (IsOwner)
            {
                position.Value = transform.position;
                rotation.Value = transform.rotation;
            }
            else
            {
                transform.position = position.Value;
                transform.rotation = rotation.Value;
            }
        }
    }
}