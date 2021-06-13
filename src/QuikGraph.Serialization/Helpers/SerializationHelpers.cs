#if SUPPORTS_GRAPHS_SERIALIZATION
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.ComponentModel;
using JetBrains.Annotations;

namespace QuikGraph.Serialization
{
    internal static class SerializationHelpers
    {
        /// <summary>
        /// Checks if the given <paramref name="type"/> is treatable (not null, <see cref="object"/> or <see cref="ValueType"/>).
        /// </summary>
        /// <param name="type"><see cref="Type"/> to check.</param>
        /// <returns>True if the <paramref name="type"/> can be treated, false otherwise.</returns>
        [Pure]
        [ContractAnnotation("type:null => false")]
        private static bool IsTreatableType([CanBeNull] Type type)
        {
            return type != null
                   && type != typeof(object)
                   && type != typeof(ValueType);
        }

        /// <summary>
        /// Checks if the given <paramref name="property"/> is an indexed one.
        /// </summary>
        /// <param name="property">A <see cref="PropertyInfo"/>.</param>
        /// <returns>True if the <paramref name="property"/> is an indexed property, false otherwise.</returns>
        [Pure]
        private static bool IsIndexed([NotNull] PropertyInfo property)
        {
            return property.GetIndexParameters().Length != 0;
        }

        /// <summary>
        /// Gets all properties that are marked with <see cref="XmlAttributeAttribute"/> on given type.
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<PropertySerializationInfo> GetAttributeProperties([CanBeNull] Type type, [CanBeNull] string prefix = null)
        {
            Type currentType = type;
            while (IsTreatableType(currentType))
            {
                // Iterate through properties that must have a get, and are not indexed property
                IEnumerable<PropertyInfo> properties = currentType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(ValidProperty);
                foreach (PropertyInfo property in properties)
                {
                    // Is it tagged with XmlAttributeAttribute
                    if (TryGetAttributeName(property, out string name))
                    {
                        if (TryGetDefaultValue(property, out object value))
                            yield return new PropertySerializationInfo(property, GetPropertyName(name), value);
                        else
                            yield return new PropertySerializationInfo(property, GetPropertyName(name));
                    }
                }

                currentType = currentType.BaseType;
            }

            #region Local functions

            bool ValidProperty(PropertyInfo property)
            {
                return property.CanRead && !IsIndexed(property);
            }

            string GetPropertyName(string baseName)
            {
                return prefix is null
                    ? baseName
                    : prefix + baseName;
            }

            #endregion
        }

        [NotNull]
        private const string TagPrefix = "TAG-";

        /// <summary>
        /// Gets all properties that are marked with <see cref="XmlAttributeAttribute"/> on given edge type.
        /// </summary>
        /// <remarks>It includes data coming from <see cref="ITagged{TTag}"/> interface implementations.</remarks>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<PropertySerializationInfo> GetEdgeAttributeProperties<TVertex, TEdge>()
            where TEdge : IEdge<TVertex>
        {
            IEnumerable<PropertySerializationInfo> properties = GetAttributeProperties(typeof(TEdge));
            IEnumerable<Type> tagTypes = typeof(TEdge).GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITagged<>))
                .Select(tagType => tagType.GetGenericArguments()[0]);

            return properties.Concat(tagTypes.SelectMany(tagType =>
            {
                PropertyInfo tagProperty = typeof(TEdge).GetProperty(nameof(ITagged<object>.Tag));
                return GetAttributeProperties(tagType, TagPrefix).Select(property =>
                {
                    property.GetTargetObject = tagProperty;
                    return property;
                });
            }));
        }

        [Pure]
        public static bool TryGetAttributeName([NotNull] PropertyInfo property, out string name)
        {
            var attribute = Attribute.GetCustomAttribute(property, typeof(XmlAttributeAttribute)) as XmlAttributeAttribute;
            if (attribute is null)
            {
                name = null;
                return false;
            }

            name = string.IsNullOrEmpty(attribute.AttributeName)
                ? property.Name
                : attribute.AttributeName;
            return true;
        }

        [Pure]
        public static bool TryGetDefaultValue([NotNull] PropertyInfo property, out object value)
        {
            var attribute = Attribute.GetCustomAttribute(property, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
            if (attribute is null)
            {
                value = null;
                return false;
            }

            value = attribute.Value;
            return true;
        }
    }
}
#endif