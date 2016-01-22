using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Protocol.VisualStudio.Services;
using Xunit;

namespace NuGet.Protocol.VisualStudio.Tests
{
    public class SearchResultsAggregatorTests
    {
        [Fact]
        public void Test()
        {
            var indexer = new LuceneSearchResultsIndexer();
            var aggregator = new SearchResultsAggregator(indexer);
        }

        private static JObject
    }
}
