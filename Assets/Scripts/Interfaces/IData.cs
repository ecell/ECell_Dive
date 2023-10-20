using ECellDive.Modules;
using ECellDive.Utility.Data.Modification;

namespace ECellDive.Interfaces
{
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