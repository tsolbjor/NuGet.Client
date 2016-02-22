using System;

namespace NuGet.ProjectModel
{
    public class LockFileSubtarget
    {
        public string RuntimeIdentifier { get; }
        public LockFileTargetLibrary Definition { get; }

        public LockFileSubtarget(string runtimeIdentifier, LockFileTargetLibrary definition)
        {
            RuntimeIdentifier = runtimeIdentifier;
            Definition = definition;
        }

        public override bool Equals(object obj)
        {
            var other = obj as LockFileSubtarget;
            return other != null &&
                string.Equals(RuntimeIdentifier, other.RuntimeIdentifier, StringComparison.OrdinalIgnoreCase) &&
                Equals(Definition, other.Definition);
        }

        public override int GetHashCode()
        {
            var combiner = new HashCodeCombiner();
            combiner.AddStringIgnoreCase(RuntimeIdentifier);
            combiner.AddObject(Definition);
            return combiner.CombinedHash;
        }
    }
}