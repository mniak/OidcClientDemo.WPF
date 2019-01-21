using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Threading.Tasks;

/* Used NuGet Packages
   install-package Microsoft.Owin
   install-package Microsoft.Owin.Host.HttpListener
   install-package Microsoft.Owin.Hosting
   install-package Owin
 */
namespace OidcClientDemo.WPF.Infrastructure
{
    public class SingleRequestListenerOLD : IDisposable
    {
        private readonly string url;
        private readonly TaskCompletionSource<IOwinRequest> completionSource;

        private IDisposable webApp;

        public SingleRequestListenerOLD(string url)
        {
            this.url = url;
            completionSource = new TaskCompletionSource<IOwinRequest>();
        }

        public Task<IOwinRequest> ListenRequest()
        {
            CheckDisposed();

            var startOptions = new StartOptions();
            startOptions.Urls.Add(url);
            webApp = WebApp.Start(startOptions, app => app.Run(HandleRequest));
            Console.WriteLine("Waiting");
            return completionSource.Task;
        }

        public static Task<IOwinRequest> ListenRequest(string url)
        {
            return new SingleRequestListenerOLD(url).ListenRequest();
        }

        private bool handled = false;
        private readonly object lockHandled = new object();
        private async Task HandleRequest(IOwinContext ctx)
        {
            lock (lockHandled)
            {
                if (handled) return;
                handled = true;
            }

            await Task.Yield();
            completionSource.SetResult(ctx.Request);
            Dispose();
        }
        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (webApp != null)
            {
                webApp.Dispose();
                disposed = true;
            }
        }
    }
}