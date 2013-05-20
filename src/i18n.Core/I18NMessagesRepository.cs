using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
                return i18NMessagesCache.Get(culture);
            }
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