﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using i18n.Parsers;

namespace i18n
{
    /// <summary>
    /// A service for retrieving localized text from PO resource files
    /// </summary>
    public class LocalizingService : ILocalizingService
    {
        private static readonly object Sync = new object();

        /// <summary>
        /// Returns the best matching language for this application's resources, based the provided languages
        /// </summary>
        /// <param name="languages">A sorted list of language preferences</param>
        public virtual string GetBestAvailableLanguageFrom(string[] languages)
        {
            foreach (var language in languages.Where(language => !string.IsNullOrWhiteSpace(language)))
            {
                var culture = GetCultureInfoFromLanguage(language);
                
                // en-US
                var result = GetLanguageIfAvailable(culture.IetfLanguageTag);
                if(result != null)
                {
                    return result;
                }

                // Save cycles processing beyond the default; this one is guaranteed
                if (culture.TwoLetterISOLanguageName.Equals(DefaultSettings.DefaultTwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
                {
                    return DefaultSettings.DefaultTwoLetterISOLanguageName;
                }

                // Don't process the same culture code again
                if (culture.IetfLanguageTag.Equals(culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // en
                result = GetLanguageIfAvailable(culture.TwoLetterISOLanguageName);
                if (result != null)
                {
                    return result;
                }
            }

            return DefaultSettings.DefaultTwoLetterISOLanguageName;
        }

        private static string GetLanguageIfAvailable(string culture)
        {
            culture = culture.ToLowerInvariant();

            var cacheKey = string.Format("po:{0}", culture);

            lock (Sync)
            {
                if (HttpRuntime.Cache[cacheKey] != null)
                {
                    // This language is already available
                    return ((List<I18NMessage>)HttpRuntime.Cache[cacheKey]).Count > 0 ? culture : null;
                }
            }

            if (LoadMessages(culture) && ((List<I18NMessage>)HttpRuntime.Cache[cacheKey]).Count > 0)
            {
                return culture;
            }
            
            // Avoid shredding the disk looking for non-existing files
            CreateEmptyMessages(culture);
            return null;
        }

        /// <summary>
        /// Returns localized text for a given default language key, or the default itself,
        /// based on the provided languages and application resources
        /// </summary>
        /// <param name="key">The default language key to search for</param>
        /// <param name="languages">A sorted list of language preferences</param>
        /// <returns></returns>
        public virtual string GetText(string key, string[] languages)
        {
            // Prefer 'en-US', then 'en', before moving to next language choice
            foreach (var language in languages.Where(language => !string.IsNullOrWhiteSpace(language)))
            {
                var culture = GetCultureInfoFromLanguage(language);

                // en-US
                var regional = TryGetTextFor(culture.IetfLanguageTag, key);

                // Save cycles processing beyond the default; just return the original key
                //comment out for #8  
                //NOTE: (tw) disabled this as in any case cultures derivant from en are ignored
                // it could be ok to use the key if the language is just en, but with this code
                // en-US en-AU en-CA are all ignored tho obviousely they could be different from 
                // the default en variable
                // next to that "en" cannot be translated using PO files which could be useful
                // Save cycles processing beyond the default; just return the original key

//                if (culture.TwoLetterISOLanguageName.Equals(DefaultSettings.DefaultTwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
//                {
//                    return key;
//                }


                // en (and regional was defined)
                if(!culture.IetfLanguageTag.Equals(culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase) && regional == key)
                {
                    var global = TryGetTextFor(culture.TwoLetterISOLanguageName, key);
                    if(global != key)
                    {
                        return global;
                    }
                    continue;
                }

                if(regional != key)
                {
                    return regional;
                }
            }

            return key;
        }

        public IList<I18NMessage> GetAllText(string language)
        {
            var culture = GetCultureInfoFromLanguage(language).IetfLanguageTag;
            var i18NMessages = (List<I18NMessage>) HttpRuntime.Cache["po:culture"];
            if (i18NMessages == null || i18NMessages.Count == 0)
            {
                LoadMessages(culture);
                i18NMessages = (List<I18NMessage>)HttpRuntime.Cache["po:culture"];
            }
            return i18NMessages;
        }

        private static string TryGetTextFor(string culture, string key)
        {
            lock (Sync)
            {
                if (HttpRuntime.Cache[string.Format("po:{0}", culture)] != null)
                {
                    // This culture is already processed and in memory
                    return GetTextOrDefault(culture, key);
                }
            }

            if(LoadMessages(culture))
            {
                return GetTextOrDefault(culture, key);    
            }
            
            // Avoid shredding the disk looking for non-existing files
            CreateEmptyMessages(culture);

            return key;
        }

        private static void CreateEmptyMessages(string culture)
        {
            lock (Sync)
            {
                string directory;
                string path;
                GetDirectoryAndPath(culture, out directory, out path);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using(var fs = File.CreateText(path))
                {
                    fs.Flush();
                }

                // If the file changes we want to be able to rebuild the index without recompiling
                HttpRuntime.Cache.Insert(string.Format("po:{0}", culture), new List<I18NMessage>(0), new CacheDependency(path));
            }
        }

        private static bool LoadMessages(string culture)
        {
            string directory;
            string path;
            GetDirectoryAndPath(culture, out directory, out path);

            if (!File.Exists(path))
            {
                return false;
            }

            LoadFromDiskAndCache(culture, path);
            return true;
        }

        private static void GetDirectoryAndPath(string culture, out string directory, out string path)
        {
            directory = string.Format("{0}/locale/{1}", HostingEnvironment.ApplicationPhysicalPath, culture);
            path = Path.Combine(directory, "messages.po");
        }

        private static void LoadFromDiskAndCache(string culture, string path)
        {
            lock (Sync)
            {
                var messages = new I18NPoFileParser().Parse(path);
                // If the file changes we want to be able to rebuild the index without recompiling
                HttpRuntime.Cache.Insert(string.Format("po:{0}", culture), messages, new CacheDependency(path));
            }
        }

        private static string GetTextOrDefault(string culture, string key)
        {
            lock (Sync)
            {
                var messages = (List<I18NMessage>) HttpRuntime.Cache[string.Format("po:{0}", culture)];

                if (!messages.Any())
                {
                    return key;
                }

                var matched = messages.SingleOrDefault(m => m.MsgId.Equals(key));

                if (matched == null)
                {
                    return key;
                }

                return string.IsNullOrWhiteSpace(matched.MsgStr) ? key : matched.MsgStr;
            }
        }

        private static CultureInfo GetCultureInfoFromLanguage(string language)
        {
            var semiColonIndex = language.IndexOf(';');
            return semiColonIndex > -1
                       ? new CultureInfo(language.Substring(0, semiColonIndex), true)
                       : new CultureInfo(language, true);
        }
    }
}

       