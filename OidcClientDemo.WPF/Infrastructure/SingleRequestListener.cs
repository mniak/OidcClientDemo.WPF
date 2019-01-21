using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OidcClientDemo.WPF.Infrastructure
{
    public class SingleRequestListener
    {
        public async static Task<T> HandleRequestAsync<T>(string url, Func<IOwinContext, CancellationToken, Task<T>> action, CancellationToken cancellationToken)
        {
            var startOptions = new StartOptions();
            startOptions.Urls.Add(url);

            var completionSource = new TaskCompletionSource<T>();
            cancellationToken.Register(() => completionSource.TrySetCanceled());
            using (var webApp = WebApp.Start(startOptions, app =>
            {
                app.Run(async ctx =>
                {
                    var result = await action(ctx, cancellationToken);
                    completionSource.SetResult(result);
                });

            }))
            {
                var result = await completionSource.Task;
                return result;
            }
        }
    }
}
