using System.Collections.Generic;
using System.Globalization;
using i18n.Core.Models;

namespace i18n.Core
{
    internal class I18NMessagesRepository
    {
        private readonly I18NMessagesCache i18NMessagesCache;

        public I18NMessagesRepository(I18NMessagesCache i18NMessagesCache)
        {
            this.i18NMessagesCache = i18NMessagesCache;
        }

        public IDictionary<string, I18NMessage> Get(string culture)
        {
            return i18NMessagesCache.Get(culture);
        }

        public string Get(CultureInfo culture, string key)
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
            var i18NMessages = Get(culture);
            if (i18NMessages.ContainsKey(key))
            {
                var i18NMessage = i18NMessages[key];
                return i18NMessage.MsgStr;
            }
            return string.Empty;
        }

        public string FindByCultures(string key, IEnumerable<CultureInfo> cultureInfos)
        {
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

        public string GetBest(IEnumerable<CultureInfo> cultureInfos, string @default)
        {
            foreach (var culture in cultureInfos)
            {
                if (Get(culture.Name).Count > 0)
                {
                    return culture.Name;
                }
                if (Get(culture.TwoLetterISOLanguageName).Count > 0)
                {
                    return culture.TwoLetterISOLanguageName;
                }
            }
            return @default;
        }
    }
}