using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Core.v3;
using NuGet.Versioning;

namespace NuGet.Protocol.VisualStudio.Converters
{
    public class PackageSearchMetadataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PackageSearchMetadata);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            var result = new PackageSearchMetadata();

            // Populate properties
            var id = jObject.GetString(Properties.PackageId);
            var version = NuGetVersion.Parse(jObject.GetString(Properties.Version));
            result.Identity = new PackageIdentity(id, version);

            var topPackage = new PackageIdentity(id, version);
            var iconUrl = jObject.GetUri(Properties.IconUrl);
            result.Description = jObject.GetString(Properties.Description);
            result.Summary = jObject.GetString(Properties.Summary);
            result.Title = jObject.GetString(Properties.Title);

            // get other versions
            var versionList = GetLazyVersionList(jObject, includePrerelease, version);

            // retrieve metadata for the top package
            UIPackageMetadata metadata = null;

            var v3MetadataResult = _metadataResource as UIMetadataResourceV3;

            // for v3 just parse the data from the search results
            if (v3MetadataResult != null)
            {
                metadata = v3MetadataResult.ParseMetadata(jObject);
            }

            // if we do not have a v3 metadata resource, request it using whatever is available
            if (metadata == null)
            {
                metadata = await _metadataResource.GetMetadata(topPackage, token);
            }

            var searchResult = new UISearchMetadata(
                topPackage,
                title,
                summary,
                string.Join(", ", metadata.Authors),
                metadata.DownloadCount,
                iconUrl,
                versionList,
                metadata);
            return result;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
