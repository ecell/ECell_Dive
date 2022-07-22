using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ECellDive.Modules;


namespace ECellDive.Interfaces
{
    public enum OperationTypes
    {
        bound,
        knockout,
    }

    /// <summary>
    /// Struct to store information about a modification file.
    /// </summary>
    public struct ModificationFile
    {
        public string author;

        public string baseModelName;
        
        public string description;

        /// <summary>
        /// The string describing all the modifications.
        /// </summary>
        public string modification;

        /// <summary>
        /// Name of the file without extension.
        /// </summary>
        public string name;

        public ModificationFile(string _name, JObject _jFileContent)
        {
            author = _jFileContent["author"].Value<string>();
            baseModelName = "";
            description = _jFileContent["description"].Value<string>();
            modification = _jFileContent["modification"].Value<string>();
            name = _name;
            ExtractModelNameFromModelPath(_jFileContent["base_model_path"].Value<string>());
        }
        
        public ModificationFile(ModificationFile _modFile)
        {
            author = _modFile.author;
            baseModelName = _modFile.baseModelName;
            description = _modFile.description;
            modification = _modFile.modification;
            name = _modFile.name;
        }

        public ModificationFile(string _author, string _baseModelName,
                                string _description, string _modification)
        {
            author = _author;
            baseModelName = _baseModelName;
            description = _description;
            modification = _modification;
            name = "newFile";
        }

        private void ExtractModelNameFromModelPath(string _modelPath)
        {
            string modelFileName = _modelPath.Split('/').Last();
            baseModelName = modelFileName.Split('.').First();
        }
    }

    public struct Modification<T>
    {
        /// <summary>
        /// The value of the data element being modified or a property of this
        /// data element as recorded in the modification file (when reading) or
        /// about to be recorded in the modification file (chen writing).
        /// </summary>
        public T value;

        /// <summary>
        /// Type of the operation associated with the modification.
        /// </summary>
        public OperationTypes opType;

        public Modification(T _value, OperationTypes _opType)
        {
            value = _value;
            opType = _opType;
        }
    }

    /// <summary>
    /// An interface to allow the data of the class to be modified
    /// by the content of modification files.
    /// </summary>
    /// <remarks>Modification files intends to be stored on a server
    /// and processed by a <see cref="HttpServerModificationModule"/></remarks>
    public interface IModifiable
    {
        ModificationFile readingModificationFile { get; set; } 

        void ApplyFileModifications();

        /// <returns>
        /// Returns True if <paramref name="_name"/> matches a name
        /// defined in the class implementing <see cref="IModifiable"/>.
        /// Returns false otherwise.
        /// </returns>
        bool CheckName(string _name);

        /// <summary>
        /// The pointing funtion used to control where to apply modifications.
        /// </summary>
        /// <param name="_operation">
        /// The string describing the operation. Elements of the 
        /// string miust be seprated by underscores. The first element must be the name
        /// of the operation. See <see cref="OperationTypes"/> for the elligible names.
        /// </param>
        void OperationSwitch(string _operation);
    }

    /// <summary>
    /// An interface to allow the modfied data of the class to be saved.
    /// </summary>
    /// <remarks>Modfications intends to be processed by a <see cref=
    /// "HttpServerModificationModule"/> and stored on a server.</remarks>
    public interface ISaveable
    {
        ModificationFile writingModificationFile { get; set; }
        void CompileModificationFile();
    }
}