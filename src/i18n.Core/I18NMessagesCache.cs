using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using i18n.Core.Models;
using i18n.Core.Parsers;

namespace i18n.Core
{
    public class I18NMessagesCache
    {
        private readonly IPoFileParser parser;
        private readonly string rootPath;
        private readonly IDictionary<string, IList<I18NMessage>> i18NMessagesCache = new Dictionary<string, IList<I18NMessage>>();

        public I18NMessagesCache(IPoFileParser parser, string rootPath)
        {
            this.parser = parser;
            this.rootPath = rootPath;
        }

        public IList<I18NMessage> Get(string culture)
        {
            if (!i18NMessagesCache.ContainsKey(culture))
            {
                UpdateI18NCache(culture);
            }
            return i18NMessagesCache[culture];
        }

        public string Get(string culture, string key)
        {
            var i18NMessage = Get(culture).FirstOrDefault(m => m.MsgId.Equals(key));
            if (i18NMessage != null && !string.IsNullOrWhiteSpace(i18NMessage.MsgStr))
            {
                return i18NMessage.MsgStr;
            }
            return key;
        }

        private void UpdateI18NCache(string culture)
        {
            var cultureBasedPoFile = Path.Combine(rootPath, "locale", culture, "messages.po");
            var result = parser.Parse(cultureBasedPoFile).ToList().Select(Selector).ToList();
            i18NMessagesCache[culture] = result;
        }

        private static Func<I18NMessage, I18NMessage> Selector
        {
            get { return message => string.IsNullOrEmpty(message.MsgStr) ? new I18NMessage(message.MsgId, message.MsgId) : message; }
        }

        public void Reset(string[] changedCultures)
        {
            changedCultures.ToList().ForEach(UpdateI18NCache);
        }
    }
}