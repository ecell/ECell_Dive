using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ECellDive.Multiplayer;

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
	/// A container to encapsulate a list of int that can be serialized
	/// by Unity Netcode (i.e., it can be passed as a parameter of a
	/// ClientRpc or ServerRpc).
	/// </summary>
	/// <remarks>
	/// We also added IEnumerable for instantiation convenience.
	/// </remarks>
	public struct ListInt32Network : IEnumerable<int>, INetworkSerializable
	{
		/// <summary>
		/// The list this container encapsulates.
		/// </summary>
		List<int> list;

		/// <summary>
		/// Shortcuts to the count of <see cref="list"/>.
		/// </summary>
		public int Count
		{
			get => list.Count;
		}

		/// <summary>
		/// Getter and setter for <see cref="list"/>.
		/// </summary>
		public List<int> Values
		{
			get => list;
			set => list = value;
		}

		/// <summary>
		/// Constructor setting the capacity of <see cref="list"/>.
		/// </summary>
		/// <param name="_nbItems">
		/// The capacity of <see cref="list"/>.
		/// </param>
		public ListInt32Network(int _nbItems)
		{
			list = new List<int>(_nbItems);
		}

		/// <summary>
		/// "Copy" constructor
		/// </summary>
		/// <param name="_list">
		/// The list to assign to <see cref="list"/>.
		/// </param>
		public ListInt32Network(List<int> _list)
		{
			list = _list;
		}

		/// <summary>
		/// Indexer for <see cref="list"/>.
		/// </summary>
		/// <param name="_index">
		/// The index of the element to get or set.
		/// </param>
		/// <returns>
		/// The element at the given index.
		/// </returns>
		public int this[int _index]
		{
			get => list[_index];
			set => list[_index] = value;
		}

		/// <summary>
		/// Convenient method to access the back of <see cref="list"/>.
		/// </summary>
		/// <returns>
		/// The last element of <see cref="list"/>.
		/// </returns>
		public int GetBack()
		{
			return list[list.Count - 1];
		}

		#region - IEnumerable<int> Methods -

		/// <summary>
		/// Adds an item to <see cref="list"/>.
		/// </summary>
		/// <param name="_item">
		/// The item to add to <see cref="list"/>.
		/// </param>
		public void Add(int _item)
		{
			list.Add(_item);
		}

		public IEnumerator<int> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region - INetworkSerializable Methods -
		/// <summary>
		/// The serialization method for <see cref="ListInt32Network"/>.
		/// Serialization by writing the count of <see cref="list"/> and then the elements
		/// to an array of int. It deserializes by reading the count and then the elements
		/// back to <see cref="list"/>.
		/// </summary>
		/// <typeparam name="TRW">
		/// An IReaderWriter type from Unity Netcode.
		/// </typeparam>
		/// <param name="serializer">
		/// Serializer from Unity Netcode.
		/// </param>
		public void NetworkSerialize<TRW>(BufferSerializer<TRW> serializer) where TRW : IReaderWriter
		{
			int count = 0;
			if (serializer.IsWriter)
			{
				count = list.Count;
			}
			serializer.SerializeValue(ref count);

			if (serializer.IsReader)
			{
				int[] arrScenes = new int[count];
				list = new List<int>(count);

				for (int n = 0; n < count; ++n)
				{
					serializer.SerializeValue(ref arrScenes[n]);
					list.Add(arrScenes[n]);
				}
			}

			if (serializer.IsWriter)
			{
				int[] arrScenes = list.ToArray();
				for (int n = 0; n < count; ++n)
				{
					serializer.SerializeValue(ref arrScenes[n]);
				}
			}
		}
		#endregion
	}


	/// <summary>
	/// A struct to store the data of a player in a multiplayer session.
	/// </summary>
	public struct PlayerNetData : INetworkSerializable
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
		/// The network serializable container for the list of scenes (IDs)
		/// the player has been in.
		/// </summary>
		public ListInt32Network scenes;

		public PlayerNetData(string _playerName, ulong _clientId, int _currentScene)
		{
			playerName = _playerName;
			clientId = _clientId;
			scenes = new ListInt32Network(_currentScene);
		}

		/// <summary>
		/// Adds the new scene id to <see cref="scenes"/>.
		/// </summary>
		/// <param name="_newSceneId">
		/// The new scene id to add to <see cref="scenes"/>.
		/// </param>
		public void AddSceneId(int _newSceneId)
		{
			scenes.Add(_newSceneId);
		}

		/// <summary>
		/// Gets the current scene id.
		/// </summary>
		/// <returns>
		/// Returns the last element of <see cref="scenes"/>.
		/// </returns>
		public int GetCurrentScene()
		{
			Debug.Log(ToString());

            return scenes[scenes.Count - 1];
		}

		/// <summary>
		/// The multiplayer serialization method.
		/// </summary>
		/// <remarks>
		/// This follows the INetworkSerializable interface from Unity Netcode for GameObject.
		/// This is necessary because this struct uses a List, which is not supported by
		/// the default serialization methods of Unity Netcode.
		/// </remarks>
		/// <typeparam name="TRW">
		/// An IReaderWriter type from Unity Netcode.
		/// </typeparam>
		/// <param name="serializer">
		/// Serializer from Unity Netcode.
		/// </param>
		public void NetworkSerialize<TRW>(BufferSerializer<TRW> serializer) where TRW : IReaderWriter
		{
			serializer.SerializeValue(ref playerName);
			serializer.SerializeValue(ref clientId);
			serializer.SerializeValue(ref scenes);
		}

		/// <summary>
		/// The ToString method for debugging purposes.
		/// </summary>
		/// <returns>
		/// The values of the struct as a readable string for debug.
		/// </returns>
		public override string ToString()
		{
            string msg = $"PlayerNetData: {playerName} (clientID: {clientId}) has visited scenes:\n{{";
			foreach (int sceneId in scenes)
			{
				  msg += $" {sceneId},";
			}
			msg += " }";
			return msg;
        }
	}
}
