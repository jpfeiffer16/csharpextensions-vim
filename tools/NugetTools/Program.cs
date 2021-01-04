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
            var input = string.Empty;
            if (args.Length > 0)
            {
                input = args[0];
            }
            else
            {
                using (var reader = new StreamReader(Console.OpenStandardInput()))
                {
                    input = reader.ReadToEnd();
                }
            }
            if (string.IsNullOrWhiteSpace(input))
            {
                // Console.Error.WriteLine("No input provided");
                Environment.Exit(0);
            }

            // Console.WriteLine(input);

            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            var resource = await repository.GetResourceAsync<PackageSearchResource>();

            var searchResults = await resource.SearchAsync(
                input,
                new SearchFilter(includePrerelease: false), 0, 10, NullLogger.Instance, CancellationToken.None);

            foreach (var result in searchResults)
            {
                var lookupResource = await repository.GetResourceAsync<FindPackageByIdResource>();
                // lookupResource.GetAllVersionsAsync
                Console.WriteLine("{0}, {1}", result.Identity.Id, result.Identity.Version.ToString());
            }
        }
    }
}
