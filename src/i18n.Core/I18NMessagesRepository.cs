using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using i18n.Core.Models;
using i18n.Core.PoParsers;

namespace i18n.Core
{
    public class I18NMessagesRepository
    {
        private static readonly object Sync = new object();
        private readonly I18NMessagesCache i18NMessagesCache;

        public I18NMessagesRepository(string rootPath)
        {
            i18NMessagesCache = new I18NMessagesCache(new I18NPoFileParser(), rootPath);
            var poFileWatcher = new PoFileWatcher(Path.Combine(rootPath, "locale"));
            poFileWatcher.OnChange += (o, e) => Reset(new ChangeListParser(e.ChangeList).GetChangedCultures());
            poFileWatcher.Begin();
        }

        public IDictionary<string,I18NMessage> Get(string culture)
        {
            lock (Sync)
            {
                var i18NMessages = i18NMessagesCache.Get(culture);
                return FillNotTranslatedMessages(i18NMessages);
            }
        }

        private static IDictionary<string, I18NMessage> FillNotTranslatedMessages(IDictionary<string, I18NMessage> i18NMessages)
        {
            var i18NMessagesResult = new Dictionary<string, I18NMessage>();

            foreach (var key in i18NMessages.Keys)
            {
                if (string.IsNullOrEmpty(i18NMessages[key].MsgStr))
                {
                    var msgId = i18NMessages[key].MsgId;
                    i18NMessagesResult.Add(key, new I18NMessage(msgId, msgId));
                }
                else
                {
                    i18NMessagesResult.Add(key, i18NMessages[key]);
                }
            }
            return i18NMessagesResult;
        }


        public void Reset(string[] changedCultures)
        {
            lock (Sync)
            {
                i18NMessagesCache.Reset(changedCultures);
            }
        }

        public string FindByCultures(string key, IEnumerable<CultureInfo> cultureInfos)
        {
            lock (Sync)
            {
                foreach (var culture in cultureInfos)
                {
                    var regional = i18NMessagesCache.Get(culture, key);
                    if (!string.IsNullOrEmpty(regional))
                    {
                        return regional;
                    }
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