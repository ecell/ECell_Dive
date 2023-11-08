using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

namespace ECellDive.Utility.Data.Multiplayer
{
    /// <summary>
    /// A container to encapsulate a list of int that can be serialized
    /// by Unity Netcode (i.e., it can be passed as a parameter of a
    /// ClientRpc or ServerRpc).
    /// </summary>
    /// <remarks>
    /// We also added IEnumerable for instantiation convenience.
    /// </remarks>
    [System.Serializable]
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

        /// <summary>
        /// Removes an item from <see cref="list"/>.
        /// </summary>
        /// <param name="_item">
        /// The item to remove from <see cref="list"/>.
        /// </param>
        public void Remove(int _item)
        {
            list.Remove(_item);
        }

        /// <summary>
        /// Converts <see cref="list"/> to an array of int. 
        /// </summary>
        /// <returns>
        /// <see cref="list"/> as an array of int.
        /// </returns>
        public int[] ToArray()
        {
            return list.ToArray();
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
	/// A container to encapsulate a list of ulong that can be serialized
	/// by Unity Netcode (i.e., it can be passed as a parameter of a
	/// ClientRpc or ServerRpc).
	/// </summary>
	/// <remarks>
	/// We also added IEnumerable for instantiation convenience.
	/// </remarks>
	[System.Serializable]
	public struct ListUInt64Network : IEnumerable<ulong>, INetworkSerializable
	{
		/// <summary>
		/// The list this container encapsulates.
		/// </summary>
		List<ulong> list;

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
		public List<ulong> Values
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
		public ListUInt64Network(int _nbItems)
		{
			list = new List<ulong>(_nbItems);
		}

		/// <summary>
		/// "Copy" constructor
		/// </summary>
		/// <param name="_list">
		/// The list to assign to <see cref="list"/>.
		/// </param>
		public ListUInt64Network(List<ulong> _list)
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
		public ulong this[int _index]
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
		public ulong GetBack()
		{
			return list[list.Count - 1];
		}

        /// <summary>
        /// Removes an item from <see cref="list"/>.
        /// </summary>
        /// <param name="_item">
        /// The item to remove from <see cref="list"/>.
        /// </param>
        public void Remove(ulong _item)
        {
            list.Remove(_item);
        }

		/// <summary>
		/// Converts <see cref="list"/> to an array of ulong. 
		/// </summary>
		/// <returns>
		/// <see cref="list"/> as an array of ulong.
		/// </returns>
		public ulong[] ToArray()
		{
			return list.ToArray();
		}

        #region - IEnumerable<ulong> Methods -

        /// <summary>
        /// Adds an item to <see cref="list"/>.
        /// </summary>
        /// <param name="_item">
        /// The item to add to <see cref="list"/>.
        /// </param>
        public void Add(ulong _item)
		{
			list.Add(_item);
		}

		public IEnumerator<ulong> GetEnumerator()
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
		/// to an array of ulong. It deserializes by reading the count and then the elements
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
				ulong[] arrScenes = new ulong[count];
				list = new List<ulong>(count);

				for (int n = 0; n < count; ++n)
				{
					serializer.SerializeValue(ref arrScenes[n]);
					list.Add(arrScenes[n]);
				}
			}

			if (serializer.IsWriter)
			{
				ulong[] arrScenes = list.ToArray();
				for (int n = 0; n < count; ++n)
				{
					serializer.SerializeValue(ref arrScenes[n]);
				}
			}
		}
		#endregion
	}
}
