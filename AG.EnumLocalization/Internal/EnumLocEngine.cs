using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AG.EnumLocalization.Internal
{
    internal static class EnumLocEngine
    {
        // [Assembly][Key][LanguageCode] = string
        private static readonly ConcurrentDictionary<Assembly, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> s_strings = new();

        // [Assembly][Key] = fallback
        private static readonly ConcurrentDictionary<Assembly, ConcurrentDictionary<string, string>> s_fallbacks = new();

        // [Assembly] = langCode
        public static readonly ConcurrentDictionary<Assembly, string> DefaultLanguages = new();

        public static string GetString(Assembly assembly, string key, string? langCode = null)
        {
            if (s_strings.TryGetValue(assembly, out var keys) && keys.TryGetValue(key, out var langs))
            {
                if (langCode is not null && langs.TryGetValue(langCode, out var result)) return result;
                if (DefaultLanguages.TryGetValue(assembly, out var defaultLangCode) && defaultLangCode != langCode && langs.TryGetValue(defaultLangCode, out result)) return result;
            }
            return GetFallback(assembly, key);
        }

        public static string GetFallback(Assembly assembly, string key)
        {
            if (s_fallbacks.TryGetValue(assembly, out var keys) && keys.TryGetValue(key, out var result)) return result;
            return key;
        }

        public static void AddOrReplaceString(Assembly assembly, string key, string langCode, string value)
        {
            s_strings.GetOrAdd(assembly, assembly => new())
                .GetOrAdd(key, key => new())[langCode] = value;
        }

        public static void AddOrReplaceFallback(Assembly assembly, string key, string fallback)
        {
            s_fallbacks.GetOrAdd(assembly, assembly => new())[key] = fallback;
        }

        public static bool RemoveString(Assembly assembly, string key, string langCode)
            => s_strings.TryGetValue(assembly, out var keys) && keys.TryGetValue(key, out var langs) && langs.TryRemove(langCode, out var _);

        public static bool RemoveStrings(Assembly assembly)
            => s_strings.TryRemove(assembly, out _);

        public static bool RemoveStrings(Assembly assembly, string key)
            => s_strings.TryGetValue(assembly, out var keys) && keys.TryRemove(key, out _);

        public static bool RemoveStringsForLang(Assembly assembly, string langCode)
        {
            var result = false;
            if (!s_strings.TryGetValue(assembly, out var keys)) return false;
            foreach (var strings in keys.Values) result |= strings.TryRemove(langCode, out _);
            return result;
        }

        public static IEnumerable<string> GetKeys(Assembly assembly)
        {
            var result = Enumerable.Empty<string>();
            if (s_strings.TryGetValue(assembly, out var keys1)) result = result.Concat(keys1.Keys);
            if (s_fallbacks.TryGetValue(assembly, out var keys2)) result = result.Concat(keys2.Keys);
            return result.Distinct();
        }

        public static IEnumerable<KeyValuePair<string, string>> GetKeysAndFallbacks(Assembly assembly)
        {
            if (s_fallbacks.TryGetValue(assembly, out var keys)) return keys;
            return [];
        }

        public static IEnumerable<KeyValuePair<string, string>> GetKeysAndStrings(Assembly assembly, string? langCode)
        {
            langCode ??= DefaultLanguages.GetValueOrDefault(assembly);
            if (langCode is not null && s_strings.TryGetValue(assembly, out var keys)) return keys.Where(kvp => kvp.Value.ContainsKey(langCode)).Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value[langCode]));
            return [];
        }

        public static IEnumerable<string> GetLangs(Assembly assembly)
        {
            if (!s_strings.TryGetValue(assembly, out var keys)) return [];
            return keys.SelectMany(kvp => kvp.Value.Keys).Distinct();
        }

        public static IEnumerable<string> GetLangs(Assembly assembly, string key)
        {
            if (!s_strings.TryGetValue(assembly, out var keys) || !keys.TryGetValue(key, out var langs)) return [];
            return langs.Keys;
        }
    }
}
