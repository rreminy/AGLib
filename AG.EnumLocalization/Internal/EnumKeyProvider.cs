using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AG.EnumLocalization.Attributes;

namespace AG.EnumLocalization.Internal
{
    internal static class EnumKeyProvider
    {
        // [Type] = prefix
        private static readonly ConcurrentDictionary<Type, string> s_prefix = new();

        // [Type][Value] = (key, fallback)
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<ulong, (string Key, string Fallback)>> s_data = new();

        // This method is unconstrained as field.GetValue(null) return an object.
        // This means we got unboxing overhead to take care of.
        public static ulong GetValue(object valueObj) // where T : struct, Enum
        {
            Debug.Assert(valueObj is Enum, "valueObj must be an enum value!");
            var value = Convert.ChangeType(valueObj, Enum.GetUnderlyingType(valueObj!.GetType()), CultureInfo.InvariantCulture);
            return
                value is sbyte sb ? (ulong)sb :
                value is byte b ? b :
                value is char c ? c : // rare
                value is short s ? (ulong)s :
                value is ushort us ? us :
                value is int i ? (ulong)i :
                value is uint ui ? ui :
                value is long l ? (ulong)l :
                value is ulong ul ? ul :
                value is nint n ? (ulong)n : // rare
                value is nuint un ? un : // rare
                throw new ArgumentException("Invalid enum type");
        }

        [SuppressMessage("Major Code Smell", "S3358", Justification = "Intended.")]
        public static void AnalyzeKeysAndFallbacks(Type type)
        {
            var prefixAttr = type.GetCustomAttributes(typeof(EnumLocStringsAttribute), false).FirstOrDefault() as EnumLocStringsAttribute;
            var prefix = s_prefix.GetOrAdd(type, GetPrefix, prefixAttr);

            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                var value = GetValue(field.GetValue(null)!); // <-- Boxing and unboxing overhead is here
                var keyAttr = field.GetCustomAttributes(typeof(EnumLocAttribute), false).FirstOrDefault() as EnumLocAttribute;
                if (prefixAttr is null && keyAttr is null) continue;
                var key =
                    keyAttr is null || keyAttr.Key is null ? Enum.GetName(type, value) :
                    string.IsNullOrEmpty(keyAttr.Key) ? null :
                    keyAttr.Key;
                if (key is null) continue;
                var fallback =
                    keyAttr is null || keyAttr.Fallback is null ? key :
                    string.IsNullOrEmpty(keyAttr.Fallback) ? string.Empty :
                    keyAttr.Fallback;
                s_data.GetOrAdd(type, _ => new())[value] = ($"{prefix}{key}", fallback);
            }
        }

        public static void AnalyzeAliases(Type type)
        {
            // ASSERT: Assumes AnalyzeKeysAndFallbacks is already invoked for all types
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                var aliasAttr = field.GetCustomAttributes(typeof(EnumLocAliasAttribute<>), false).FirstOrDefault();
                if (aliasAttr is null) continue;
                var attrType = aliasAttr.GetType();

                var aliasValueObj = attrType.GetProperty(nameof(EnumLocAliasAttribute<>.Value), BindingFlags.Public | BindingFlags.Instance)!.GetValue(aliasAttr);
                if (aliasValueObj is null) continue;
                var aliasValue = GetValue(aliasValueObj);
                var aliasType = attrType.GetGenericArguments()[0];

                var key = GetKey(aliasType, aliasValue);
                var value = GetValue(field.GetValue(null)!);
                var fallback = s_data.GetOrAdd(aliasType, _ => new()).GetValueOrDefault(aliasValue).Fallback ?? key;

                s_data.GetOrAdd(type, _ => new())[value] = ($"{key}", fallback);
            }
        }

        public static IEnumerable<(string Key, string Fallback)> GetKeysAndFallbacks(Type type)
        {
            if (!s_data.TryGetValue(type, out var values)) return [];
            return values.Values;
        }

        public static string GetKey(Type type, ulong value)
        {
            if (s_data.TryGetValue(type, out var values) && values.TryGetValue(value, out var result)) return result.Key;
            var prefix = s_prefix.GetOrAdd(type, GetPrefix);
            var key = Enum.GetName(type, value);
            return $"{prefix}{key}";
        }

        private static string GetPrefix(Type type)
        {
            var prefixAttr = type.GetCustomAttributes(typeof(EnumLocStringsAttribute), false).FirstOrDefault() as EnumLocStringsAttribute;
            return GetPrefix(type, prefixAttr);
        }

        [SuppressMessage("Major Code Smell", "S3358", Justification = "Intended.")]
        private static string GetPrefix(Type type, EnumLocStringsAttribute? prefixAttr)
            => prefixAttr is null || prefixAttr.KeyPrefix is null ? $"{type.Name}." :
                string.IsNullOrEmpty(prefixAttr.KeyPrefix) ? string.Empty :
                $"{prefixAttr.KeyPrefix}.";
    }
}
