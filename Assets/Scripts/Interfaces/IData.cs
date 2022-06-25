using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ECellDive.Interfaces
{
    public enum Operations
    {
        bound,
        knockout,
    }

    /// <summary>
    /// An interface to allow the data of the class to be modified
    /// by the content of modification files.
    /// </summary>
    /// <remarks>Modification files intends to be stored on a server
    /// and processed by a <see cref="HttpServerModificationModule"/></remarks>
    public interface IModifiable
    {
        void ApplyFileModifications(string _modificationContent);

        /// <returns>
        /// Returns True if <paramref name="_name"/> matches a name
        /// defined in the class implementing <see cref="IModifiable"/>.
        /// Returns false otherwise.
        /// </returns>
        bool CheckName(string _name);

        void OperationSwitch(string _operation);
    }

    /// <summary>
    /// An interface to allow the modfied data of the class to be saved.
    /// </summary>
    /// <remarks>Modfications intends to be processed by a <see cref=
    /// "HttpServerModificationModule"/> and stored on a server.</remarks>
    public interface ISaveable
    {
        void SaveModifications();
    }
}