using System.Collections.Generic;
using UnityEngine;

namespace ECellDive.Utility
{
	/// <summary>
	/// A utility class to manipulate arrays. The purpose is to be able to fragment an array
	/// to a 2 dimensional array or a list of arrays and to assemble them back.
	/// This is useful to send data over the multiplayer network.
	/// </summary>
	public static class ArrayManipulation
	{
		/// <summary>
		/// Assemble a 2 dimensional array into a 1 dimensional array.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the array.
		/// </typeparam>
		/// <param name="_fragments">
		/// The 2 dimensional array to assemble where a "fragment" is a 1 dimensional array.
		/// </param>
		/// <returns>
		/// The assembled array.
		/// </returns>
		public static T[] Assemble<T>(T[][] _fragments)
		{
			int wholeSize = 0;
			foreach (T[] _frag in _fragments)
			{
				wholeSize += _frag.Length;
			}

			T[] assembly = new T[wholeSize];

			int nbAddedElem = 0;
			foreach (T[] _frag in _fragments)
			{
				for (int i = 0; i < _frag.Length; i++)
				{
					assembly[i+nbAddedElem] = _frag[i];
						
				}
				nbAddedElem += _frag.Length;
			}

			return assembly;
		}

		/// <summary>
		/// Assemble a list of arrays into a 1 dimensional array.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the array.
		/// </typeparam>
		/// <param name="_fragments">
		/// The list of arrays to assemble.
		/// </param>
		/// <returns>
		/// The assembled array.
		/// </returns>
		public static T[] Assemble<T>(List<T[]> _fragments)
		{
			int wholeSize = 0;
			foreach (T[] _frag in _fragments)
			{
				wholeSize += _frag.Length;
			}

			T[] assembly = new T[wholeSize];

			int nbAddedElem = 0;
			foreach (T[] _frag in _fragments)
			{
				for (int i = 0; i < _frag.Length; i++)
				{
					assembly[i + nbAddedElem] = _frag[i];

				}
				nbAddedElem += _frag.Length;
			}

			return assembly;
		}

		/// <summary>
		/// Fragment an array into a list of arrays.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the array.
		/// </typeparam>
		/// <param name="_source">
		/// The array to fragment.
		/// </param>
		/// <param name="_fragmentSize">
		/// The number of elements in each fragment.
		/// </param>
		/// <returns>
		/// The list of fragments. The last fragment may be smaller than the others.
		/// </returns>
		public static List<T[]> FragmentToList<T>(T[] _source, ushort _fragmentSize)
		{
			List<T[]> sourceFrag = new List<T[]>();

			int elements_remaining = _source.Length;
			int nbFrag = 0;
			while (elements_remaining > 0)
			{
				int nbElements = Mathf.Min(_fragmentSize, elements_remaining);
				T[] frag = new T[nbElements];

				for (int i = 0; i < nbElements; i++)
				{
					frag[i] = _source[i + nbFrag * _fragmentSize];
				}

				sourceFrag.Add(frag);
				elements_remaining -= nbElements;
				nbFrag++;
			}

			return sourceFrag;
		}

		/// <summary>
		/// Fragment an array into a 2 dimensional array.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the array.
		/// </typeparam>
		/// <param name="_source">
		/// The array to fragment.
		/// </param>
		/// <param name="_fragmentSize">
		/// The number of elements in each fragment.
		/// </param>
		/// <returns>
		/// The 2 dimensional array of fragments. The last fragment may be smaller than the others.
		/// </returns>
		public static T[][] FragmentToArray<T>(T[] _source, ushort _fragmentSize)
		{
			T[][] sourceFrag = new T[Mathf.CeilToInt(_source.Length / _fragmentSize) + 1][];

			int elements_remaining = _source.Length;
			int nbFrag = 0;
			while (elements_remaining > 0)
			{
				int nbElements = Mathf.Min(_fragmentSize, elements_remaining);
				T[] frag = new T[nbElements];

				for (int i = 0; i < nbElements; i++)
				{
					frag[i] = _source[i + nbFrag * _fragmentSize];
				}

				sourceFrag[nbFrag] = frag;
				elements_remaining -= nbElements;
				nbFrag++;
			}

			return sourceFrag;
		}
	}
}