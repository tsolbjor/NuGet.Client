using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Xunit;

namespace NuGet.Common.Test
{
    public class StringResourceTests
    {
        private static readonly object SharedCultureLock = new object();
        private static CultureInfo SharedCulture = null;

        [Theory]
        [InlineData(typeof(PublicResource))]
        [InlineData(typeof(InternalResource))]
        [InlineData(typeof(PrivateResource))]
        public void StringResource_AcceptsResources(Type type)
        {
            // Arrange
            lock (SharedCultureLock)
            {
                SharedCulture = null;
                StringResource actual;

                // Act & Assert
                var success = StringResource.TryCreate(type, out actual);

                Assert.True(success, $"The type {type.FullName} should be treated as a resource.");
                Assert.Equal(type, actual.Type);

                actual.Culture = CultureInfo.InvariantCulture;
                Assert.Same(CultureInfo.InvariantCulture, actual.Culture);
                Assert.Same(CultureInfo.InvariantCulture, SharedCulture);

                actual.Culture = null;
                Assert.Null(actual.Culture);
                Assert.Null(SharedCulture);
            }
        }

        [Fact]
        public void StringResource_WorksOnCommon()
        {
            // Arrange
            var assembly = typeof(StringResource).GetTypeInfo().Assembly;

            // Act
            var actual = StringResource.GetFromAssembly(assembly).ToArray();

            // Assert
            Assert.Equal(1, actual.Length);
            Assert.Equal("NuGet.Common.Strings", actual[0].Type.FullName);
        }

        [Theory]
        [InlineData(typeof(MissingResourceManager))]
        [InlineData(typeof(MissingSetter))]
        [InlineData(typeof(MissingGetter))]
        [InlineData(typeof(NonStaticResourceManager))]
        [InlineData(typeof(NonStaticCulture))]
        [InlineData(typeof(MissingCulture))]
        [InlineData(typeof(MissingCulture))]
        public void StringResource_RejectsNonResources(Type type)
        {
            // Arrange
            StringResource actual;

            // Act
            var success = StringResource.TryCreate(type, out actual);

            // Assert
            Assert.False(success, $"The type {type.FullName} should not be treated as a resource.");
            Assert.Null(actual);
        }

        private static class PublicResource
        {
            public static ResourceManager ResourceManager { get; set; }
            public static CultureInfo Culture
            {
                get { return SharedCulture; }
                set { SharedCulture = value; }
            }
        }

        private static class InternalResource
        {
            internal static ResourceManager ResourceManager { get; set; }
            internal static CultureInfo Culture
            {
                get { return SharedCulture; }
                set { SharedCulture = value; }
            }
        }

        private static class PrivateResource
        {
            private static ResourceManager ResourceManager { get; set; }
            private static CultureInfo Culture
            {
                get { return SharedCulture; }
                set { SharedCulture = value; }
            }
        }

        private static class MissingResourceManager
        {
            public static CultureInfo Culture { get; set; }
        }

        private static class MissingSetter
        {
            public static ResourceManager ResourceManager { get; set; }
            public static CultureInfo Culture { get; }
        }

        private static class MissingGetter
        {
            private static CultureInfo _culture;
            public static ResourceManager ResourceManager { get; set; }
            public static CultureInfo Culture { set { _culture = value; } }
        }

        private class NonStaticResourceManager
        {
            public ResourceManager ResourceManager { get; set; }
            public static CultureInfo Culture { get; set; }
        }

        private class NonStaticCulture
        {
            public static ResourceManager ResourceManager { get; set; }
            public CultureInfo Culture { get; set; }
        }

        private static class MissingCulture
        {
            public static ResourceManager ResourceManager { get; set; }
        }
    }
}
