#if SUPPORTS_GRAPHS_SERIALIZATION
using System.Reflection;
using JetBrains.Annotations;

namespace QuikGraph.Serialization
{
    internal sealed class PropertySerializationInfo
    {
        /// <summary>
        /// Gets the embedded <see cref="PropertyInfo"/>.
        /// </summary>
        [NotNull]
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        [NotNull]
        public string Name { get; }

        private readonly bool _hasDefaultValue;

        [CanBeNull]
        private readonly object _defaultValue;

        /// <summary>
        /// <see cref="PropertyInfo"/> to get object holding <see cref="Property"/>.
        /// </summary>
        /// <remarks>Only relevant for property in sub object.</remarks>
        [CanBeNull]
        public PropertyInfo GetTargetObject { get; set; }

        public PropertySerializationInfo([NotNull] PropertyInfo property, [NotNull] string name)
            : this(property, name, null)
        {
        }

        public PropertySerializationInfo(
            [NotNull] PropertyInfo property,
            [NotNull] string name,
            [CanBeNull] object defaultValue)
        {
            Property = property;
            Name = name;
            _defaultValue = defaultValue;
            _hasDefaultValue = _defaultValue != null;
        }

        [Pure]
        public bool TryGetDefaultValue(out object value)
        {
            value = _defaultValue;
            return _hasDefaultValue;
        }
    }
}
#endif