using System.Diagnostics.Tracing;

namespace NuGet.Common
{
    /// <summary>
    /// A minimal event source for logging ETW events.
    /// </summary>
    public class NuGetEventSource : EventSource
    {
        public void Load(long ImageBase, string Name) { WriteEvent(1, ImageBase, Name); }

        /// <summary>
        /// Static instance of this class.
        /// </summary>
        public static NuGetEventSource Log { get; } = new NuGetEventSource();
    }
}
