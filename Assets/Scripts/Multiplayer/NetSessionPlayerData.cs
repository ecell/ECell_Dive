using ECellDive.Interfaces;

namespace ECellDive.Interfaces
{
    public interface ISessionPlayerData
    {
        string playerName { get; set; }
        ulong clientId { get; set; }
        //bool IsConnected { get; set; }
        int currentScene { get; set; }
        //void Reinitialize();

        void SetSceneId(int _newSceneId);
    }
}

namespace ECellDive.Multiplayer
{
    public struct NetSessionPlayerData : ISessionPlayerData
    {
        #region ISessionPlayerData Members -
        public string playerName { get; set; }
        public ulong clientId { get; set; }
        public int currentScene { get; set; }
        //public bool HasCharacterSpawned;

        //public bool IsConnected { get; set; }
        
        #endregion

        public NetSessionPlayerData(string _playerName, ulong _clientId, int _currentScene)
        {
            playerName = _playerName;
            clientId = _clientId;
            currentScene = _currentScene;
        }

        #region - ISessionPlayerData Methods -
        //public void Reinitialize()
        //{
        //    HasCharacterSpawned = false;
        //}

        public void SetSceneId(int _newSceneId)
        {
            currentScene = _newSceneId;
        }

        #endregion
    }
}
