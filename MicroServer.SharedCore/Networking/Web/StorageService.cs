using System;
using System.IO;
using System.Collections.Generic;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using MicroServer.Networking.Web.Files;
using Windows.Storage.FileProperties;
using System.Diagnostics;

namespace MicroServer.Networking.Web
{
    public class StorageService : IFileService
    {
        private readonly StorageFolder _installLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
        private readonly string _rootFolder;
        private readonly string _rootUri;

        public StorageService(string rootUri, string rootFolderPath)
        {
            if (rootUri == null) throw new ArgumentNullException("rootUri");
            if (rootFolderPath == null) throw new ArgumentNullException("rootFolderPath");
            if (_installLocation.TryGetItemAsync(rootFolderPath).GetResults() == null)
                throw new ArgumentOutOfRangeException("rootFolderPath", rootFolderPath,
                                                      "Failed to find path " + rootFolderPath);
            _rootUri = rootUri;
            _rootFolder = rootFolderPath;
        }

        private string GetFullPath(Uri uri)
        {
            if (!uri.AbsolutePath.StartsWith(_rootUri))
                return null;

            var relativeUri = Uri.UnescapeDataString(uri.AbsolutePath.Remove(0, _rootUri.Length));
            return string.Concat(_rootFolder, @"\", relativeUri.TrimStart('/').Replace('/', '\\'));
        }

        public async void GetFileAsync(FileContext context)
        {
            string fullPath = @"WebRoot\index.html";

            IInputStream content;
            StorageFile file;

            try
            {
                file = await _installLocation.GetFileAsync(fullPath) as StorageFile;
                content = await file.OpenSequentialReadAsync();
                //IInputStream stream = content.GetInputStreamAt(0);
                context.SetFile(file, content, DateTime.MaxValue);
                content.Dispose();

            }
            catch (FileNotFoundException)
            {
                context.SetFile(null, null, DateTime.MaxValue);
                return;
            }
        }

        //private async Task<bool> GetFileAsync(FileContext context)
        //{
        //    string fullPath = @"WebRoot\index.html";

        //    StorageFile file = await _installLocation.TryGetItemAsync(fullPath) as StorageFile;
        //    if (file == null)
        //        return false;

        //    BasicProperties basicProperties = await file.GetBasicPropertiesAsync();
        //    Debug.WriteLine(String.Format("File size: {0} bytes", basicProperties.Size));
        //    Debug.WriteLine(String.Format("Date modified: {0}", basicProperties.DateModified));

        //    DateTime date = basicProperties.DateModified.UtcDateTime;

        //    // browser ignores second fractions.
        //    date = date.AddTicks(-(date.Ticks % TimeSpan.TicksPerSecond));
        //    if (date <= context.BrowserCacheDate)
        //    {
        //        context.SetNotModified(file, date);
        //        return true;
        //    }

        //    try
        //    {
        //        IRandomAccessStreamWithContentType content = await file.OpenReadAsync();

        //        if (content != null)
        //        {
        //            IInputStream stream = content.GetInputStreamAt(0);
        //            context.SetFile(file, stream, DateTime.MaxValue);
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //        return true;
        //    }
        //    catch (FileNotFoundException)
        //    {
        //        context.SetFile(null, null, DateTime.MaxValue);
        //        return false;
        //    }
        //}

        public IEnumerable<StorageFile> GetFiles(Uri uri)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StorageFolder> GetFolders(Uri uri)
        {
            throw new NotImplementedException();
        }

        public bool IsDirectory(Uri uri)
        {
            throw new NotImplementedException();
        }



        //private async Task WriteResponseAsync(string path, IOutputStream os)
        //{
        //    using (Stream resp = os.AsStreamForWrite())
        //    {
        //        bool exists = true;
        //        try
        //        {
        //            // Look in the Data subdirectory of the app package
        //            string filePath = "Data" + path.Replace('/', '\\');

        //            using (Stream fs = await LocalFolder.OpenStreamForReadAsync(filePath))
        //            {
        //                string header = String.Format("HTTP/1.1 200 OK\r\n" +
        //                                "Content-Length: {0}\r\n" +
        //                                "Connection: close\r\n\r\n",
        //                                fs.Length);
        //                byte[] headerArray = Encoding.UTF8.GetBytes(header);
        //                await resp.WriteAsync(headerArray, 0, headerArray.Length);
        //                await fs.CopyToAsync(resp);
        //            }
        //        }
        //        catch (FileNotFoundException)
        //        {
        //            exists = false;
        //        }

        //        if (!exists)
        //        {
        //            byte[] headerArray = Encoding.UTF8.GetBytes(
        //                                  "HTTP/1.1 404 Not Found\r\n" +
        //                                  "Content-Length:0\r\n" +
        //                                  "Connection: close\r\n\r\n");
        //            await resp.WriteAsync(headerArray, 0, headerArray.Length);
        //        }

        //        await resp.FlushAsync();
        //    }
        //}

        //public bool GetFile(FileContext context)
        //{
        //    string fullPath = @"WebRoot\index.html";

        //    StorageFile file = _installLocation.TryGetItemAsync(fullPath).GetResults() as StorageFile;
        //    if (file == null)
        //        return false;

        //    BasicProperties basicProperties = file.GetBasicPropertiesAsync().GetResults();
        //    Debug.WriteLine(String.Format("File size: {0} bytes", basicProperties.Size));
        //    Debug.WriteLine(String.Format("Date modified: {0}", basicProperties.DateModified));

        //    try
        //    {
        //        using (IRandomAccessStream content = file.OpenAsync(FileAccessMode.Read).GetResults())
        //        {
        //            if (content != null)
        //            {
        //                context.SetFile(file, content.GetInputStreamAt(0), DateTime.MaxValue);
        //                return true;
        //            }
        //            return false;
        //        }
        //    }
        //    catch (FileNotFoundException)
        //    {
        //        context.SetFile(null, null, DateTime.MaxValue);
        //        return false;
        //    }
        //}

        //public bool TryGetFile(FileContext context)
        //{
        //    string fullPath = GetFullPath(context.RouteUri);
        //    if (string.IsNullOrEmpty(fullPath))
        //        return false;

        //    fullPath = @"WebRoot\\index.html";

        //    StorageFile file = _installLocation.TryGetItemAsync(fullPath).GetResults() as StorageFile;
        //    if (file == null)
        //        return false;

        //    BasicProperties tom = file.GetBasicPropertiesAsync().GetResults();

        //    DateTime date = file.GetBasicPropertiesAsync().GetResults().DateModified.UtcDateTime;

        //    DateTime date = DateTime.MaxValue;

        //    // browser ignores second fractions.
        //    date = date.AddTicks(-(date.Ticks % TimeSpan.TicksPerSecond));
        //    if (date <= context.BrowserCacheDate)
        //    {
        //        context.SetNotModified(file, date);
        //        return true;
        //    }

        //    try
        //    {
        //        IInputStream content = file.OpenSequentialReadAsync().GetResults();
        //        if (content != null)
        //        {
        //            context.SetFile(file, content, date);
        //            return true;
        //        }
        //    }
        //    catch (FileNotFoundException)
        //    {
        //        context.SetFile(null, null, DateTime.MaxValue);
        //        return false;
        //    }
        //    return false;
        //}

    }
}
