using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ECellDive.Modules;
using UnityEngine;

namespace ECellDive.Interfaces
{
    public struct Modification
    {
        /// <summary>
        /// The name of the author of the modification file.
        /// </summary>
        public string author;

        /// <summary>
        /// The date when the modification was saved.
        /// </summary>
        public string date;

        /// <summary>
        /// The commands associated with the modification
        /// </summary>
        public List<string[]> commands;

        /// <summary>
        /// This constructor is usefull when reading a modification from a
        /// <see cref="ModificationFile"/>.
        /// </summary>
        /// <param name="_modification">The JObject read from the original Modification
        /// File (Json format).</param>
        public Modification(JObject _modification)
        {
            author = _modification["author"].Value<string>();
            date = _modification["date"].Value<string>();

            commands = new List<string[]>();
            JArray Jcmds = (JArray)_modification["commands"];
            string[] cmd;
            foreach(JArray Jcmd in Jcmds)
            {
                cmd = new string[Jcmd.Count];
                for (int i = 0; i<Jcmd.Count; i++)
                {
                    Debug.Log(Jcmd.ElementAt(i).Value<string>());
                    cmd[i] = Jcmd.ElementAt(i).Value<string>();
                }
                commands.Add(cmd);
            }
        }

        /// <summary>
        /// This constructor is usefull when about to store additional modifications
        /// in a <see cref="ModificationFile"/>.
        /// </summary>
        /// <param name="_author">The value for <see cref="author"/></param>
        /// <param name="_date">The value for <see cref="author"/></param>
        /// <param name="_commands">All the commands associated with the
        /// modification in the format:
        /// Cmd1-Param1-...-ParamN &amp; ... &amp; CmdM-Param1-...-ParamL.</param>
        public Modification(string _author, string _date, List<string> _commands)
        {
            author = _author;
            date = _date;

            commands = new List<string[]>();
            foreach (string _cmd in _commands)
            {
                commands.Add(_cmd.Split('-'));
            }
        }

        /// <summary>
        /// Gets the string version of all the commands stored in <see cref="commands"/>.
        /// </summary>
        /// <returns>
        /// Returns all the commands in the format: Cmd1-Param1-...-ParamN &amp; ... &amp; CmdM-Param1-...-ParamL
        /// </returns>
        public string[] GetCommands()
        {
            string[] strCommands = new string[commands.Count];
            for(int i = 0; i < commands.Count; i++)
            {
                string strCommand = commands[i][0];
                for (int j = 1; j < commands[i].Length; j++)
                {
                    strCommand += "-" + commands[i][j];
                }
                strCommands[i] = strCommand;
            }
            return strCommands;
        }
    }

    /// <summary>
    /// Struct to store information about a modification file.
    /// </summary>
    public struct ModificationFile
    {
        /// <summary>
        /// The name of the model file this modification file is
        /// applied to.
        /// </summary>
        public string baseModelName;

        /// <summary>
        /// The list of all modifications stored in the file.
        /// </summary>
        public List<Modification> modifications;

        /// <summary>
        /// Name of the file without extension.
        /// </summary>
        public string name;

        /// <summary>
        /// Constructor usefull when reading a Modification file (Json format).
        /// </summary>
        /// <param name="_name">The name of the file</param>
        /// <param name="_jFileContent">The JObject build after parsing the
        /// modification file.</param>
        public ModificationFile(string _name, JObject _jFileContent)
        {
            name = _name;
            baseModelName = _jFileContent["base_model_name"].Value<string>();
            modifications = new List<Modification>();
            JArray modificationList = (JArray)_jFileContent["modification_list"];

            foreach (JObject _mod in modificationList)
            {
                modifications.Add(new Modification(_mod));
            }
        }

        /// <summary>
        /// Constructor usefull when building a modification file from within
        /// ECellDive before, probably, saving it as a Json file.
        /// </summary>
        /// <param name="_baseModelName">The value for <see cref="baseModelName"/>.</param>
        /// <param name="_modification">The modification of the file to be saved.</param>
        public ModificationFile(string _baseModelName, Modification _modification)
        {
            name = "newFile";
            baseModelName = _baseModelName;
            modifications = new List<Modification>() { _modification};
        }

        /// <summary>
        /// Concatenates all the commands from the whole list <see cref="modifications"/>
        /// </summary>
        /// <returns>Returns all the commands in the format:
        /// Cmd1-Param1-...-ParamN &amp; ... &amp; CmdM-Param1-...-ParamL</returns>
        public List<string[]> GetAllCommands()
        {
            List<string[]> allCommands = new List<string[]>();
            for (int i = 0; i < modifications.Count; i++)
            {
                allCommands.Add(modifications[i].GetCommands());
            }

            return allCommands;
        }

        /// <summary>
        /// Gets the author of the modification at index <paramref name="_modIndex"/> in
        /// <see cref="modifications"/> if it exists.
        /// </summary>
        /// <param name="_modIndex">The index of the modification in <see cref="modifications"/>
        /// for which we want to get the author.</param>
        /// <returns>Returns the author if it exists. Otherwise, returns an empty string.</returns>
        public string GetAuthorOfMod(int _modIndex)
        {
            string author = "";
            if (_modIndex < modifications.Count)
            {
                author = modifications[_modIndex].author;
            }
            return author;
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
        /// <summary>
        /// The struct to store the information about the object being modified.
        /// </summary>
        ModificationFile readingModificationFile { get; set; } 

        /// <summary>
        /// Applies all modifications stored in <see cref="readingModificationFile"/> to the loaded module.
        /// </summary>
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
        /// string must be seprated by underscores. The first element must be the name
        /// of the operation.
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
        /// <summary>
        /// Stores the information about the modification file 
        /// we wish to save.
        /// </summary>
        ModificationFile writingModificationFile { get; set; }

        /// <summary>
        /// Builds <see cref="writingModificationFile"/>.
        /// </summary>
        void CompileModificationFile();
    }
}