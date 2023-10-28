using UnityEngine;

namespace ECellDive.PlayerComponents
{
	/// <summary>
	/// Singleton class which saves/loads local-client settings.
	/// (This is just a wrapper around the PlayerPrefs system,
	/// so that all the calls are in the same place.)
	/// </summary>
	public static class PlayerPrefsWrap
	{
		public static string GetPlayerName()
		{
			return PlayerPrefs.GetString("PlayerName", "Player");
		}

		public static void SetPlayerName(string name)
		{
			PlayerPrefs.SetString("PlayerName", name);
		}

		//const float k_DefaultMasterVolume = 0.5f;
		//const float k_DefaultMusicVolume = 0.8f;

		//public static float GetMasterVolume()
		//{
		//    return PlayerPrefs.GetFloat("MasterVolume", k_DefaultMasterVolume);
		//}

		//public static void SetMasterVolume(float volume)
		//{
		//    PlayerPrefs.SetFloat("MasterVolume", volume);
		//}

		//public static float GetMusicVolume()
		//{
		//    return PlayerPrefs.GetFloat("MusicVolume", k_DefaultMusicVolume);
		//}

		//public static void SetMusicVolume(float volume)
		//{
		//    PlayerPrefs.SetFloat("MusicVolume", volume);
		//}

		/// <summary>
		/// Either loads a Guid string from Unity preferences, or creates one and checkpoints it, then returns it.
		/// </summary>
		/// <returns>
		/// The Guid that uniquely identifies this client install, in string form.
		/// </returns>
		public static string GetGUID()
		{
#if !UNITY_EDITOR // Don't save GUIDs in editor, so that we can test multiple clients on one machine.
			if (PlayerPrefs.HasKey("client_guid"))
			{
				return PlayerPrefs.GetString("client_guid");
			}
#endif
			System.Guid guid = System.Guid.NewGuid();
			string guidString = guid.ToString();

			PlayerPrefs.SetString("client_guid", guidString);
			return guidString;
		}

	}
}

