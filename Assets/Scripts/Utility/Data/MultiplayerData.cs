using System;

namespace ECellDive.Utility.Data.Multiplayer
{
    /// <summary>
    /// A struct to store the essential data of a player to send to a server
    /// so that the player can be authorized.
    /// </summary>
    [Serializable]
    public struct ConnectionPayload
    {
        /// <summary>
        /// The id of a player.
        /// </summary>
        /// <remarks>
        /// It might be compared to banned, muted, etc. lists.
        /// </remarks>
        public string playerId;

        /// <summary>
        /// The password for the server.
        /// </summary>
        /// <remarks>
        /// WARNING: Should be encrypted. (Not implemented yet)
        /// </remarks>
        public string psw;

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string playerName;
    }

    /// <summary>
    /// Connection information for the multiplayer server.
    /// </summary>
    [Serializable]
    public struct ConnectionSettings
    {
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string playerName;

        /// <summary>
        /// The IPv4 address of the multiplayer server.
        /// </summary>
        public string IP;

        /// <summary>
        /// The port of the multiplayer server.
        /// </summary>
        public ushort port;

        /// <summary>
        /// The password for the multiplayer server.
        /// </summary>
        public string password;

        /// <summary>
        /// Setter for <see cref="playerName"/>.
        /// </summary>
        /// <param name="_playerName">
        /// The new value for <see cref="playerName"/>.
        /// </param>
        public void SetPlayerName(string _playerName)
        {
            playerName = _playerName;
        }

        /// <summary>
        /// Setter for <see cref="IP"/>.
        /// </summary>
        /// <param name="_IP">
        /// The new value for <see cref="IP"/>.
        /// </param>
        public void SetIP(string _IP)
        {
            IP = _IP;
        }

        /// <summary>
        /// Setter for <see cref="port"/>.
        /// </summary>
        /// <param name="_port">
        /// The new value for <see cref="port"/>.
        /// </param>
        public void SetPort(ushort _port)
        {
            port = _port;
        }

        /// <summary>
        /// Setter for <see cref="password"/>.
        /// </summary>
        /// <param name="_password">
        /// The new value for <see cref="password"/>.
        /// </param>
        public void SetPassword(string _password)
        {
            password = _password;
        }
    }

    /// <summary>
    /// A struct to store the data of a player in a multiplayer session.
    /// </summary>
    public struct NetSessionPlayerData
	{
		/// <summary>
		/// The name of the player.
		/// </summary>
		public string playerName;

		/// <summary>
		/// The client id of the player.
		/// </summary>
		public ulong clientId;

		/// <summary>
		/// The id of the scene the player is currently in.
		/// </summary>
		public int currentScene;

		public NetSessionPlayerData(string _playerName, ulong _clientId, int _currentScene)
		{
			playerName = _playerName;
			clientId = _clientId;
			currentScene = _currentScene;
		}
		
		/// <summary>
		/// Sets the value of <see cref="currentScene"/>.
		/// </summary>
		/// <param name="_newSceneId">
		/// The new value for <see cref="currentScene"/>.
		/// </param>
		public void SetSceneId(int _newSceneId)
		{
			currentScene = _newSceneId;
		}
	}
}
