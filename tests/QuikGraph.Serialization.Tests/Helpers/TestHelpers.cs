using System;
using JetBrains.Annotations;

namespace QuikGraph.Serialization.Tests
{
    /// <summary>
    /// Various helpers for Serialization tests.
    /// </summary>
    internal static class TestHelpers
    {
        /// <summary>
        /// Converts a <see cref="Type"/> into a string representation for easy serialization.
        /// </summary>
        [Pure]
        [NotNull]
        public static string TypeToSerializableType([NotNull] Type type)
        {
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }

        [Pure]
        [NotNull]
        public static string VertexIdentity_Simple(int vertex)
        {
            return vertex.ToString();
        }

        [Pure]
        [NotNull]
        public static string VertexIdentity_Complex([NotNull] EquatableTestVertex vertex)
        {
            return vertex.ID;
        }
    }
}