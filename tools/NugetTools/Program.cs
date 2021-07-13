using System;
using System.IO;
using System.Threading;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NugetTools
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string input;
            if (args.Length > 0)
            {
                input = args[0];
            }
            else
            {
                using var reader = new StreamReader(Console.OpenStandardInput());
                input = await reader.ReadToEndAsync();
            }
            if (string.IsNullOrWhiteSpace(input))
            {
                Environment.Exit(0);
            }

            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            var resource = await repository.GetResourceAsync<PackageSearchResource>();

            using var searchTokenSource = new CancellationTokenSource(3000/*30 seconds*/);
            var searchResults = await resource.SearchAsync(
                input,
                new SearchFilter(includePrerelease: false), 0, 10, NullLogger.Instance, searchTokenSource.Token);

            foreach (var result in searchResults)
            {
                Console.WriteLine("{0}, {1}", result.Identity.Id, result.Identity.Version.ToString());
            }
        }
    }
}
