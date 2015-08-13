using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.ApplicationModel.Background;

using MicroServer.Logging;
using MicroServer.Logging.Loggers;
using MicroServer.Logging.Providers;

using MicroServer.Networking.Web;


namespace MicroServer
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral backgroundTaskDeferral = null;

        private SystemDebugLogger systemDebugLogger = new SystemDebugLogger(typeof(StartupTask));
        private LogProvider logProvider = new LogProvider();

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the deferral and save it to local variable so that the app stays alive
            this.backgroundTaskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskCanceled;

            //  Setup system logging
            logProvider.Add(systemDebugLogger, new NamespaceFilter("MicroServer"));
            LogManager.Provider = logProvider;
            ILogger Logger = LogManager.GetLogger<StartupTask>();

            try
            {
                // Create Http server pipeline  
                ModuleManager ModuleManager = new ModuleManager();

                // Add the router module as the fist module to pipeline
                ModuleManager.Add(new RouterModule());

                // Add the storage service module to pipeline
                //ModuleManager.Add(new FileModule(new StorageService("/", @"WebRoot")));

                // Add the controller module to pipeline
                ModuleManager.Add(new ControllerModule());

                // Add the error module as the last module to pipeline
                ModuleManager.Add(new ErrorModule());

                //  Create the http server
                HttpServer webServer = new HttpServer(ModuleManager);

                IAsyncAction asyncAction = ThreadPool.RunAsync(
                    async (workItem) =>
                    {
                        await webServer.StartAsync("80");
                    });

                Logger.Info("Background task is running...");

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in Run: {0}", ex.Message);
            }
        }
        private void TaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Release the deferral so that the app can be stopped.
            this.backgroundTaskDeferral.Complete();
        }
    }
}
