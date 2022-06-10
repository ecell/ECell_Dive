using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ECellDive.Utility
{
    public static class ArrayManipulation
    {
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
                    assembly[i+nbAddedElem] = _frag[i];
                        
                }
                nbAddedElem += _frag.Length;
            }

            return assembly;
        }

        public static List<T[]> Fragment<T>(T[] _source, ushort _fragmentSize)
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
    }
}