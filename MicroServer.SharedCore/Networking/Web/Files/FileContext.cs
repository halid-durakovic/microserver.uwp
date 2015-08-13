using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MicroServer.Networking.Web.Files
{
    /// <summary>
    /// Context used by <see cref="IFileService"/> when locating files.
    /// </summary>
    /// <remarks>
    /// There are three scenarios for files:
    /// <list type="table">
    /// <item>
    /// <term>Not found</term>
    /// <description>Simply do not change the context, just return from the method.</description>
    /// </item>
    /// <item>
    /// <term>Found but not modified.</term>
    /// <description>The file UTC date/time is less or equal to <see cref="FileContext.BrowserCacheDate"/>. Use <see cref="FileContext.SetNotModified"/> and return</description>
    /// </item>
    /// <item>
    /// <term>Found and newer</term>
    /// <description>The file UTC date/time is newer than <see cref="FileContext.BrowserCacheDate"/>. Use <see cref="FileContext.SetFile"/> and return.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public class FileContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileContext"/> class.
        /// </summary>
        /// <param name="routeUri">The request.</param>
        /// <param name="browserCacheDate">Usually the header "If-Modified-Since"</param>
        public FileContext(Uri routeUri , DateTime browserCacheDate)
        {
            if (routeUri == null) throw new ArgumentNullException("routeUri");
            RouteUri = routeUri;
            BrowserCacheDate = browserCacheDate;
        }

        /// <summary>
        /// Gets the request (the Uri specifies the wanted file) stored for the route.
        /// </summary>
        /// <remarks>For instance used to convert the URI into parameters.</remarks>
        /// <seealso cref="RouterModule"/>
        public Uri RouteUri { get; private set; }

        /// <summary>
        /// Gets date when file was cached in the browser.
        /// </summary>
        public DateTime BrowserCacheDate { get; set; }

        /// <summary>
        /// Gets if file was found;
        /// </summary>
        /// <remarks>The stream is not set if the file was found but not modified.</remarks>
        public bool IsFound { get; private set; }

        /// <summary>
        /// Gets if file was modified since it was last requested;
        /// </summary>
        public bool IsModified { get; private set; }
        
        /// <summary>
        /// Gets the date when the file was modified (UTC time)
        /// </summary>
        public DateTime LastModified { get; private set; }

        /// <summary>
        /// Gets file stream
        /// </summary>
        /// <remarks>The server will own the stream</remarks>
        public StorageFile File { get; private set; }

        /// <summary>
        /// Gets file stream
        /// </summary>
        /// <remarks>The server will own the stream</remarks>
        public IInputStream FileContent { get; private set; }

        /// <summary>
        /// Set file that should be returned.
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="stream">File stream</param>
        /// <param name="lastModifiedAtUtc">When the file was modified (UTC time).</param>
        /// <remarks>
        /// <para>The stream will be disposed by the server after it's being sent</para>
        /// <para>Use <see cref="SetNotModified"/> if the file has not been modified</para>
        /// </remarks>
        public void SetFile(StorageFile file, IInputStream fileContent, DateTime lastModified)
        {
            if (fileContent == null) throw new ArgumentNullException("stream");

            File = file;
            LastModified = lastModified;
            FileContent = fileContent;
            IsFound = true;
            IsModified = true;
        }

        /// <summary>
        /// File has not been modified.
        /// </summary>
        /// <param name="file">File name including extension.</param>
        /// <param name="date"></param>
        public void SetNotModified(StorageFile file, DateTime date)
        {
            if (file == null) throw new ArgumentNullException("file");
            File = file;
            LastModified = date;
            IsFound = true;
            IsModified = false;
        }
    }
}