using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using i18n.Core.Models;
using i18n.Core.PoParsers;

namespace i18n.Core
{
    internal class LocalizingService : ILocalizingService
    {
        private readonly GenericCache<IDictionary<string, I18NMessage>> genericCache;
        private readonly string localePath;
        private readonly IPoFileParser poFileParse;
        private const string DefaultTwoLetterISOLanguageName = "en";

        internal LocalizingService(GenericCache<IDictionary<string, I18NMessage>> genericCache, string localePath, IPoFileParser poFileParse)
        {
            this.genericCache = genericCache;
            this.localePath = localePath;
            this.poFileParse = poFileParse;
        }

        private IDictionary<string, I18NMessage> GetOrParse(string culture)
        {
            return genericCache.Get(culture, () => poFileParse.Parse(Path.Combine(localePath, "locale", culture, "messages.po")));
        }

        private string Get(CultureInfo culture, string key)
        {
            var msg = GetByCultureStr(culture.Name, key);
            if (string.IsNullOrEmpty(msg))
            {
                msg = GetByCultureStr(culture.TwoLetterISOLanguageName, key);
            }
            return msg;
        }

        private string GetByCultureStr(string culture, string key)
        {
            var i18NMessages = GetOrParse(culture);
            if (i18NMessages.ContainsKey(key))
            {
                var i18NMessage = i18NMessages[key];
                return i18NMessage.MsgStr;
            }
            return string.Empty;
        }

        private static IEnumerable<CultureInfo> ToCultureInfos(IEnumerable<string> languages)
        {
            return languages.Where(language => !String.IsNullOrWhiteSpace(language)).Select(language1 =>
                {
                    var semiColonIndex = language1.IndexOf(';');
                    var l = semiColonIndex < 0 ? language1: language1.Substring(0, semiColonIndex);
                    return new CultureInfo(CultureInfo.CreateSpecificCulture(l).Name, true);
                });
        }

        public string GetBestAvailableLanguageFrom(string[] languages)
        {
            var cultureInfos = ToCultureInfos(languages);
            foreach (var culture in cultureInfos)
            {
                if (GetOrParse(culture.Name).Count > 0)
                {
                    return culture.Name;
                }
                if (GetOrParse(culture.TwoLetterISOLanguageName).Count > 0)
                {
                    return culture.TwoLetterISOLanguageName;
                }
            }
            return DefaultTwoLetterISOLanguageName;
        }

        public string GetText(string key, string[] languages)
        {
            var cultureInfos = ToCultureInfos(languages);
            foreach (var culture in cultureInfos)
            {
                var regional = Get(culture, key);
                if (!string.IsNullOrEmpty(regional))
                {
                    return regional;
                }
            }
            return key;
        }

        public IList<I18NMessage> GetAll(string[] languages)
        {
            var culture = GetBestAvailableLanguageFrom(languages);
            return GetOrParse(culture)
                .Select(p => new I18NMessage(p.Key, string.IsNullOrEmpty(p.Value.MsgStr)? p.Key: p.Value.MsgStr))
                .ToList();
        }
    }
}
