using System.Globalization;
using System.Resources;
using System.Threading;

namespace NuGet
{
    internal static class LocalizedResourceManager
    {
        public static CultureInfo Culture { get; set; }
        public static ResourceManager ResourceManager { get; private set; } =
            new ResourceManager("NuGet.CommandLine.NuGetResources", typeof(LocalizedResourceManager).Assembly);

        public static string GetString(string resourceName)
        {
            var culture = GetLanguageName();
            return ResourceManager.GetString(resourceName + '_' + culture, CultureInfo.InvariantCulture) ??
                   ResourceManager.GetString(resourceName, CultureInfo.InvariantCulture);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "the convention is to used lower case letter for language name.")]        
        /// <summary>
        /// Returns the 3 letter language name used to locate localized resources.
        /// </summary>
        /// <returns>the 3 letter language name used to locate localized resources.</returns>
        public static string GetLanguageName()
        {
            CultureInfo culture = Culture;
            if (culture == null)
            {
                culture = Thread.CurrentThread.CurrentUICulture;
                while (!culture.IsNeutralCulture)
                {
                    if (culture.Parent == culture)
                    {
                        break;
                    }

                    culture = culture.Parent;
                }
            }

            return culture.ThreeLetterWindowsLanguageName.ToLowerInvariant();
        }
    }
}
