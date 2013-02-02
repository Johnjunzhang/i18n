using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using i18n.Core;
using i18n.Core.Models;

namespace i18n
{
    /// <summary>
    /// A service for retrieving localized text from PO resource files
    /// </summary>
    public class LocalizingService : ILocalizingService
    {
        private readonly I18NMessagesRepository i18NMessagesRepository;

        public LocalizingService(I18NMessagesRepository i18NMessagesRepository)
        {
            this.i18NMessagesRepository = i18NMessagesRepository;
        }

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

        private string GetLanguageIfAvailable(string culture)
        {
            var messages = i18NMessagesRepository.Get(culture);
            return messages.Count > 0 ? culture : null;
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
                var regional = i18NMessagesRepository.GetTextFor(culture.IetfLanguageTag, key);

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
                    var global = i18NMessagesRepository.GetTextFor(culture.TwoLetterISOLanguageName, key);
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

        public IList<I18NMessage> GetAll(string[] language)
        {
            var culture = GetBestAvailableLanguageFrom(language);
            return i18NMessagesRepository.Get(culture);
        }

        private static CultureInfo GetCultureInfoFromLanguage(string language)
        {
            var semiColonIndex = language.IndexOf(';');
            language = semiColonIndex > -1 ? language.Substring(0, semiColonIndex) : language;
            language = CultureInfo.CreateSpecificCulture(language).Name;
            return new CultureInfo(language, true);
        }
    }
}

       