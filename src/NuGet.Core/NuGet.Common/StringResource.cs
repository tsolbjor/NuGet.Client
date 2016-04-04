using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
#if NETSTANDARD1_5
using System.Runtime.Loader;
#endif

namespace NuGet.Common
{
    public class StringResource
    {
        private static readonly Assembly ThisAssembly = typeof(StringResource).GetTypeInfo().Assembly;

        private static readonly ISet<string> AssemblyExtensions = new HashSet<string>(
            new[] { ".dll", ".exe" },
            StringComparer.OrdinalIgnoreCase);

        private readonly MethodInfo _getCultureMethod;
        private readonly MethodInfo _setCultureMethod;

        private StringResource(Type type, MethodInfo getCultureMethod, MethodInfo setCultureMethod)
        {
            Type = type;
            _getCultureMethod = getCultureMethod;
            _setCultureMethod = setCultureMethod;
        }

        public Type Type { get; }

        public CultureInfo Culture
        {
            get { return (CultureInfo)_getCultureMethod.Invoke(null, new object[0]); }
            set { _setCultureMethod.Invoke(null, new[] { value }); }
        }

        public static CultureInfo ResourceCulture { get; private set; } = CultureInfo.CurrentCulture;

        public static bool TryCreate(Type type, out StringResource resource)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var typeInfo = type.GetTypeInfo();

            // The type must have a static ResourceManager property.
            var resourceManager = typeInfo.GetProperty(
                "ResourceManager",
                BindingFlags.NonPublic | BindingFlags.Public |
                BindingFlags.Static);

            if (resourceManager == null || resourceManager.PropertyType != typeof(ResourceManager))
            {
                resource = null;
                return false;
            }

            // The type must have a static Culture property with a getter and setter.
            var culture = typeInfo.GetProperty(
                "Culture",
                BindingFlags.NonPublic | BindingFlags.Public |
                BindingFlags.Static);

            if (culture == null || culture.PropertyType != typeof(CultureInfo))
            {
                resource = null;
                return false;
            }

            var getMethod = culture.GetGetMethod(true);
            var setMethod = culture.GetSetMethod(true);

            if (getMethod == null || setMethod == null)
            {
                resource = null;
                return false;
            }

            resource = new StringResource(type, getMethod, setMethod);
            return true;
        }

        public static IEnumerable<StringResource> GetFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly
                .GetTypes()
                .Select(type =>
                {
                    // Try to treat the type as a resource, which is characterized by its available properties.
                    StringResource resource;
                    StringResource.TryCreate(type, out resource);
                    return resource;
                })
                .Where(resource => resource != null);
        }

        public static IReadOnlyList<Type> DisableLocalizationInNuGetResources()
        {
            var resources = SetCulture(
                "NuGet*",
                type => type.Namespace != null && type.Namespace.StartsWith("NuGet"),
                CultureInfo.InvariantCulture);

            return resources.ToList();
        }

        private static IEnumerable<Type> SetCulture(string searchPattern, Func<Type, bool> shouldSetCulture, CultureInfo culture)
        {
            // Set the defaults (e.g. String.Format).
            ResourceCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Set the culture on the resources.
            foreach (var resource in GetFromMatchingAssemblies(searchPattern))
            {
                if (shouldSetCulture(resource.Type))
                {
                    resource.Culture = culture;
                    yield return resource.Type;
                }
            }
        }

        private static IEnumerable<StringResource> GetFromMatchingAssemblies(string searchPattern)
        {
            // Get all of the assemblies to search in.
            IEnumerable<Assembly> loadedAssemblies;
#if NETSTANDARD1_5
            loadedAssemblies = Enumerable.Empty<Assembly>();
#else
            loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
#endif

            var assemblies = loadedAssemblies
                .Concat(LoadSiblingAssemblies(searchPattern))
                .Distinct();

            // Get all of the resources from the assemblies.
            return assemblies
                .SelectMany(GetFromAssembly);
        }

        private static IEnumerable<Assembly> LoadSiblingAssemblies(string searchPattern)
        {
            var directory = Path.GetDirectoryName(ThisAssembly.Location);
            var assemblies = new HashSet<Assembly>();

            foreach (var path in Directory.EnumerateFiles(directory, searchPattern, SearchOption.TopDirectoryOnly))
            {
                Console.WriteLine("All: " + path);
            }

            foreach (var path in Directory.EnumerateFiles(directory, searchPattern, SearchOption.TopDirectoryOnly))
            {
                if (!AssemblyExtensions.Contains(Path.GetExtension(path)))
                {
                    continue;
                }

                Console.WriteLine("Looking at: " + path);

                Assembly assembly = null;
                try
                {
                    assembly = LoadAssemblyFromPath(path);
                }
                catch
                {
                    // Ignore assemblies that cannot be loaded.
                }

                if (assembly != null &&
                    HasValidPublicKey(assembly) &&
                    assemblies.Add(assembly))
                {
                    yield return assembly;
                }
            }
        }

        private static bool HasValidPublicKey(Assembly assembly)
        {
            var thisPublicKey = ThisAssembly.GetName().GetPublicKey();
            var otherPublicKey = assembly.GetName().GetPublicKey();

            if (thisPublicKey == null)
            {
                return otherPublicKey == null;
            }

            return thisPublicKey.SequenceEqual(otherPublicKey);
        }

        private static Assembly LoadAssemblyFromPath(string path)
        {
#if NETSTANDARD1_5
            return AssemblyLoadContext.GetLoadContext(ThisAssembly).LoadFromAssemblyPath(path);
#else
            return Assembly.LoadFile(path);
#endif
        }
    }
}
