using NuGet.Protocol.Core.Types;
using System.Collections.Generic;

namespace NuGet.Protocol.VisualStudio.Services
{
    public interface ISearchResultsAggregator
    {
        IEnumerable<PackageSearchMetadata> Aggregate(string queryString, params IEnumerable<PackageSearchMetadata>[] results);
    }
}
