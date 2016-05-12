using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace NuGet.Packaging.Core
{
    /// <summary>
    /// Until NuspecReader and Manifest are unified, this is a place to share implementations of
    /// reading and parsing specific elements out of the .nuspec XML.
    /// </summary>
    public static class NuspecUtility
    {
        public static readonly string PackageTypes = "packageTypes";
        public static readonly string PackageType = "packageType";
        public static readonly string PackageTypeName = "name";
        public static readonly string PackageTypeVersion = "version";

        /// <summary>
        /// Gets zero or more package types from the .nuspec.
        /// </summary>
        public static IReadOnlyList<PackageType> GetPackageTypes(XElement metadataNode, bool useMetadataNamespace)
        {
            IEnumerable<XElement> nodes;
            if (useMetadataNamespace)
            {
                var metadataNamespace = metadataNode.GetDefaultNamespace().NamespaceName;
                nodes = metadataNode
                    .Elements(XName.Get(PackageTypes, metadataNamespace))
                    .SelectMany(x => x.Elements(XName.Get(PackageType, metadataNamespace)));
            }
            else
            {
                nodes = metadataNode
                    .Elements()
                    .Where(x => x.Name.LocalName == PackageTypes)
                    .SelectMany(x => x.Elements())
                    .Where(x => x.Name.LocalName == PackageType);
            }

            var packageTypes = new List<PackageType>();
            foreach (var node in nodes)
            {
                // Get the required package type name.
                var nameAttribute = node.Attribute(XName.Get(PackageTypeName));

                if (nameAttribute == null || string.IsNullOrWhiteSpace(nameAttribute.Value))
                {
                    throw new PackagingException(Strings.MissingPackageTypeName);
                }

                var name = nameAttribute.Value.Trim();

                // Get the optional package type version.
                var versionAttribute = node.Attribute(XName.Get(PackageTypeVersion));
                Version version;

                if (versionAttribute != null)
                {
                    if (!Version.TryParse(versionAttribute.Value, out version))
                    {
                        throw new PackagingException(string.Format(
                            CultureInfo.CurrentCulture,
                            Strings.InvalidPackageTypeVersion,
                            versionAttribute.Value));
                    }
                }
                else
                {
                    version = Core.PackageType.EmptyVersion;
                }

                packageTypes.Add(new PackageType(name, version));
            }

            return packageTypes;
        }
    }
}
