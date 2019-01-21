using System;
using System.Diagnostics;

namespace OidcClientDemo.WPF.Infrastructure
{
    class Browser : IDisposable
    {
        private readonly Process process;

        private Browser(Process process)
        {
            this.process = process;
        }
        internal static Browser Navigate(string authorizeUrl)
        {
            var psi = new ProcessStartInfo(authorizeUrl)
            {
                UseShellExecute = true,
            };
            var process = Process.Start(psi);
            return new Browser(process);
        }

        public void Dispose()
        {
            this.Close();
        }

        internal void Close()
        {
            process.Close();
            process.Dispose();
        }
    }
}