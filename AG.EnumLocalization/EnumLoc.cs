using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AG.EnumLocalization.Internal;

namespace AG.EnumLocalization
{
    /// <summary>Provides localization support using enums.</summary>
    /// <remarks>All methods are thread-safe.</remarks>
    public static class EnumLoc
    {
        /// <summary>Sets the default <paramref name="langCode"/> for a specific <paramref name="assembly"/>.</summary>
        /// <param name="langCode">Language code.</param>
        /// <param name="assembly"><see cref="Assembly"/>.</param>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.PreserveSig)]
        public static void SetDefaultLanguage(string langCode, Assembly? assembly = null)
            => EnumLocEngine.DefaultLanguages[assembly ?? Assembly.GetCallingAssembly()] = langCode;

        /// <summary>Gets the default language code for a specific <paramref name="assembly"/>.</summary>
        /// <param name="assembly"><see cref="Assembly"/>.</param>
        /// <returns>Language code.</returns>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.PreserveSig)]
        public static string? GetDefaultLanguage(Assembly? assembly = null)
            => EnumLocEngine.DefaultLanguages.GetValueOrDefault(assembly ?? Assembly.GetCallingAssembly());

        /// <summary>Sets up a specific <paramref name="assembly"/> for localization.</summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>This method needs to be run first for each <paramref name="assembly"/>.</item>
        /// <item>This also resets all loaded localization strings for <paramref name="assembly"/>.</item>
        /// </list>
        /// </remarks>
        /// <param name="assembly"><see cref="Assembly"/>.</param>
        /// <param name="debug">Use keys instead of fallback text for fallbacks.</param>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.PreserveSig)]
        public static void SetupAssembly(Assembly? assembly = null, bool debug = false)
        {
            assembly ??= Assembly.GetCallingAssembly();
            EnumLocEngine.RemoveStrings(assembly);

            var types = assembly.GetTypes()
                .Where(type => type.IsEnum)
                .ToList();

            foreach (var type in types) EnumKeyProvider.AnalyzeKeysAndFallbacks(type);
            foreach (var type in types) EnumKeyProvider.AnalyzeAliases(type);
            foreach (var type in types)
            {
                var data = EnumKeyProvider.GetKeysAndFallbacks(type);
                foreach (var (key, fallback) in data)
                {
                    EnumLocEngine.AddOrReplaceFallback(assembly, key, debug ? key : fallback);
                }
            }
        }

        /// <summary>Loads a specific <paramref name="langCode"/> for a specified <paramref name="assembly"/> from <paramref name="data"/>.</summary>
        /// <param name="langCode">Language code.</param>
        /// <param name="data">Data to read from. Can be a <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="assembly"><see cref="Assembly"/>.</param>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.PreserveSig)]
        public static void SetupLanguage(string langCode, IEnumerable<KeyValuePair<string, string>> data, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            EnumLocEngine.RemoveStringsForLang(assembly, langCode);
            foreach (var (key, value) in data) EnumLocEngine.AddOrReplaceString(assembly, key, langCode, value);
        }

        /// <summary>Gets all keys with their fallbacks for a specified <paramref name="assembly"/>.</summary>
        /// <param name="assembly"><see cref="Assembly"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the keys and fallbacks as <see cref="KeyValuePair{TKey, TValue}"/>.</returns>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.PreserveSig)]
        public static IEnumerable<KeyValuePair<string, string>> GetKeysAndFallbacks(Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            return EnumLocEngine.GetKeysAndFallbacks(assembly);
        }

        /// <summary>Gets all keys with their strings for a specified <paramref name="assembly"/>'s <paramref name="langCode"/>.</summary>
        /// <param name="langCode">Language code.</param>
        /// <param name="assembly"><see cref="Assembly"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the keys and strings as <see cref="KeyValuePair{TKey, TValue}"/>.</returns>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.PreserveSig)]
        public static IEnumerable<KeyValuePair<string, string>> GetKeysAndStrings(string? langCode = null, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            return EnumLocEngine.GetKeysAndStrings(assembly, langCode);
        }

        /// <summary>Get the enum localization key for this specific enum value.</summary>
        /// <typeparam name="T">Enum <see cref="Type"/>.</typeparam>
        /// <param name="value">Enum value.</param>
        /// <returns>Enum localization key.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLocKey<T>(this T value) where T : struct, Enum
        {
            var type = typeof(T);
            return EnumKeyProvider.GetKey(type, EnumKeyProvider.GetValue(value));
        }

        /// <summary>Get the enum localization string for this specific enum value for <paramref name="langCode"/>.</summary>
        /// <typeparam name="T">Enum <see cref="Type"/>.</typeparam>
        /// <param name="value">Enum value.</param>
        /// <param name="langCode">Language code.</param>
        /// <returns>Enum localized string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLocString<T>(this T value, string? langCode = null) where T : struct, Enum
        {
            var key = value.GetLocKey();
            return EnumLocEngine.GetString(typeof(T).Assembly, key, langCode);
        }

        /// <summary>Get the enum localization fallback for this specific enum value.</summary>
        /// <typeparam name="T">Enum <see cref="Type"/>.</typeparam>
        /// <param name="value">Enum value.</param>
        /// <returns>Enum localization fallback string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLocFallback<T>(this T value) where T : struct, Enum
        {
            var key = value.GetLocKey();
            return EnumLocEngine.GetFallback(typeof(T).Assembly, key);
        }
    }
}
