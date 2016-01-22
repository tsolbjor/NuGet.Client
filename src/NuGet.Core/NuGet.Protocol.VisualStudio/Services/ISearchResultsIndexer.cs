using NuGet.Protocol.Core.Types;
using System.Collections.Generic;

namespace NuGet.Protocol.VisualStudio.Services
{
    public interface ISearchResultsIndexer
    {
        IDictionary<string, int> Rank(string queryString, IEnumerable<PackageSearchMetadata> entries);
    }
}
