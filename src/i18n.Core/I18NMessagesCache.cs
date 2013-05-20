using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using i18n.Core.Models;
using i18n.Core.PoParsers;

namespace i18n.Core
{
    public class I18NMessagesCache
    {
        private readonly IPoFileParser parser;
        private readonly string rootPath;
        private readonly IDictionary<string, IDictionary<string, I18NMessage>> i18NMessagesCache = 
            new Dictionary<string, IDictionary<string, I18NMessage>>(StringComparer.InvariantCultureIgnoreCase);

        public I18NMessagesCache(IPoFileParser parser, string rootPath)
        {
            this.parser = parser;
            this.rootPath = rootPath;
        }

        public IDictionary<string, I18NMessage> Get(string culture)
        {
            if (!i18NMessagesCache.ContainsKey(culture))
            {
                UpdateI18NCache(culture);
            }
            return i18NMessagesCache[culture];
        }

        private string GetByCultureStr(string culture, string key)
        {
            var cultureMessages = Get(culture);
            if (cultureMessages.ContainsKey(key))
            {
                var i18NMessage = cultureMessages[key];
                return i18NMessage.MsgStr;
            }
            return string.Empty;
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

        private void UpdateI18NCache(string culture)
        {
            var cultureBasedPoFile = Path.Combine(rootPath, "locale", culture, "messages.po");
            var result = parser.Parse(cultureBasedPoFile);
            i18NMessagesCache[culture] = result;
        }

        public void Reset(string[] changedCultures)
        {
            changedCultures.ToList().ForEach(UpdateI18NCache);
        }
    }
}