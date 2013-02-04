using System.Collections.Generic;
using System.IO;
using i18n.Core.Models;
using i18n.Core.Parsers;

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

        public string GetTextFor(string culture, string key)
        {
            lock (Sync)
            {
                return i18NMessagesCache.Get(culture, key);
            }
        }

        public IList<I18NMessage> Get(string culture)
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
    }
}