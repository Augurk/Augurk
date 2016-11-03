using Augurk.CommandLine.Options;
using System;
using System.Net.Http;
using System.Text;

namespace Augurk.CommandLine.Plumbing
{
    internal static class AugurkHttpClientFactory
    {
        public static HttpClient CreateHttpClient(SharedOptions options)
        {
            if (options.UseIntegratedSecurity)
            {
                var username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                Console.WriteLine($"Using integrated security with user {username} to access the Augurk API's.");

                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true,
                    PreAuthenticate = true,
                };
                var secureClient = new HttpClient(handler);
                return secureClient;
            }

            var client = new HttpClient();

            if (options.UseBasicAuthentication)
            {
                if (string.IsNullOrEmpty(options.BasicAuthenticationUsername) || string.IsNullOrEmpty(options.BasicAuthenticationPassword))
                {
                    Console.Error.WriteLine("When using basic HTTP authentication, you must specify a username and password)");
                    System.Environment.Exit(-1);
                }

                var byteArray = Encoding.ASCII.GetBytes($"{options.BasicAuthenticationUsername}:{options.BasicAuthenticationPassword}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            return client;
        }
    }
}
