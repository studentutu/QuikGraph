using System;
#if SUPPORTS_SERIALIZATION
using System.Runtime.Serialization;
#endif
using JetBrains.Annotations;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz font.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class GraphvizFont
#if SUPPORTS_SERIALIZATION
        : IDeserializationCallback
#endif
    {
        /// <summary>
        /// Font name.
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontname">See more</see>
        /// </summary>
        [NotNull]
        public string Name { get; }

        /// <summary>
        /// Font size (in points).
        /// <see href="https://www.graphviz.org/doc/info/attrs.html#d:fontsize">See more</see>
        /// </summary>
        public float SizeInPoints { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphvizFont"/> class.
        /// </summary>
        /// <param name="name">Font name.</param>
        /// <param name="sizeInPoints">Font size.</param>
        public GraphvizFont([NotNull] string name, float sizeInPoints)
        {
            ValidateContract(name, sizeInPoints);

            Name = name;
            SizeInPoints = sizeInPoints;
        }

        [AssertionMethod]
        private static void ValidateContract([NotNull] string name, float sizeInPoints)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Font name cannot be null or empty.", nameof(name));
            if (sizeInPoints <= 0)
                throw new ArgumentOutOfRangeException(nameof(sizeInPoints), "Size must be positive.");
        }

#if SUPPORTS_SERIALIZATION
        #region IDeserializationCallback

        /// <inheritdoc />
        void IDeserializationCallback.OnDeserialization(object sender)
        {
            ValidateContract(Name, SizeInPoints);
        }

        #endregion
#endif
    }
}