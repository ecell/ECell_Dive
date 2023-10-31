using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using ECellDive.Utility;


namespace ECellDive.SceneManagement
{
	/// <summary>
	/// The logic to handle switching between different scenes assets.
	/// </summary>
	public class AssetScenesManager : NetworkBehaviour
	{
		/// <summary>
		/// The array of scenes names that might be loaded
		/// </summary>
		public string[] scenePaths;

		/// <summary>
		/// The dictionary identifying the loaded scenes by their name in <see cref="scenePaths"/>.
		/// This is used to unload the scenes.
		/// </summary>
		private Dictionary<string, Scene> loadedScenes = new Dictionary<string, Scene>();

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
		}

		/// <summary>
		///  Calls UnityEngine.SceneManagement.SceneManager.LoadScene(string)  to loads the
		///  scene at index <paramref name="_sceneIdx"/> in  UnityEngine.SceneManagement.Scene.
		/// </summary>
		/// <param name="_sceneIdx">The index of the scene we wish to load as ordered in
		/// <see cref="scenePaths"/></param>
		public void LoadScene(int _sceneIdx)
		{
			if (IsServer && NetworkManager.Singleton.ConnectedClientsIds.Count == 1)
			{
				SceneEventProgressStatus status = NetworkManager.SceneManager.LoadScene(
														scenePaths[_sceneIdx], LoadSceneMode.Additive);
			}
		}

		/// <summary>
		/// Subscribed to Unity.Netcode.NetworkManager.SceneManager.OnSceneEvent.
		/// Used to control instructions based on the nature of the event.
		/// </summary>
		/// <param name="_sceneEvent">The class containing information about the scene.</param>
		/// <remarks>
		/// Copied and adapted from documentation (2022-08-15):
		/// <see href="https://docs-multiplayer.unity3d.com/netcode/current/basics/scenemanagement/using-networkscenemanager#unloading-a-scene/"/>
		/// </remarks>
		private void OnSceneEvent(SceneEvent _sceneEvent)
		{
			switch (_sceneEvent.SceneEventType)
			{
				case SceneEventType.LoadComplete:
					{
						// We want to handle this for only the server-side
						if (_sceneEvent.ClientId == NetworkManager.ServerClientId)
						{
							// *** IMPORTANT ***
							// Keep track of the loaded scene, you need this to unload it
							loadedScenes[_sceneEvent.SceneName] = _sceneEvent.Scene;
						}
						//Debug.Log($"Loaded the {_sceneEvent.SceneName} scene on " +
						//	$"{_sceneEvent.ClientId}.");
						break;
					}
				case SceneEventType.UnloadComplete:
					{
						//Debug.Log($"Unloaded the {_sceneEvent.SceneName} scene on " +
						//	$"{_sceneEvent.ClientId}.");
						break;
					}
				case SceneEventType.LoadEventCompleted:
				case SceneEventType.UnloadEventCompleted:
					{
						var loadUnload = _sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted ? "Load" : "Unload";
						//Debug.Log($"{loadUnload} event completed for the following client " +
						//	$"identifiers:({_sceneEvent.ClientsThatCompleted})");

						if (_sceneEvent.ClientsThatTimedOut.Count > 0)
						{
							Debug.LogWarning($"{loadUnload} event timed out for the following client " +
								$"identifiers:({_sceneEvent.ClientsThatTimedOut})");
						}
						break;
					}
			}
		}

		/// <summary>
		/// Unloads the scene which name is at index <paramref name="_sceneIdx"/> in
		/// <see cref="scenePaths"/>.
		/// </summary>
		/// <param name="_sceneIdx">
		/// The index of the scene we wish to unload as ordered in <see cref="scenePaths"/>.
		/// </param>
		public void UnloadScene(int _sceneIdx)
		{
			if (IsServer && NetworkManager.Singleton.ConnectedClientsIds.Count == 1)
			{
				Scene scene;
				if (loadedScenes.TryGetValue(scenePaths[_sceneIdx], out scene))
				{
					NetworkManager.SceneManager.UnloadScene(scene);
				}
			}
		}
	}
}

