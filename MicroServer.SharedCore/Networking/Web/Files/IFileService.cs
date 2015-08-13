using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace MicroServer.Networking.Web.Files
{
    /// <summary>
    /// Serves files
    /// </summary>
    public interface IFileService
    {

        /// <summary>
        /// Get a file
        /// </summary>
        /// <param name="context">Context used to locate and return files</param>
        /// <remarks><c>true</c> if the file was attached to the response; otherwise false;</remarks>
        void GetFileAsync(FileContext context);

        /// <summary>
        /// Gets if the specified url corresponds to a directory serving files
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns></returns>
        bool IsDirectory(Uri uri);

        /// <summary>
        /// Get all files that exists in the specified directory
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns></returns>
        IEnumerable<StorageFile> GetFiles(Uri uri);

        /// <summary>
        /// Gets a list of all sub directores 
        /// </summary>
        /// <param name="uri">URI (as requested by the HTTP client) which should correspond to a folder.</param>
        /// <returns></returns>
        IEnumerable<StorageFolder> GetFolders(Uri uri);
    }
}