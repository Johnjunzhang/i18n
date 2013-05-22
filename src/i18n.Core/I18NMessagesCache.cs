using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using i18n.Core.Models;
using i18n.Core.PoParsers;

namespace i18n.Core
{
    internal class I18NMessagesCache
    {
        private readonly IPoFileParser parser;
        private readonly string rootPath;
        private readonly IDictionary<string, IDictionary<string, I18NMessage>> i18NMessagesCache = 
            new Dictionary<string, IDictionary<string, I18NMessage>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly object sync = new object();

        public I18NMessagesCache(IPoFileParser parser, string rootPath)
        {
            this.parser = parser;
            this.rootPath = rootPath;
        }

        public IDictionary<string, I18NMessage> Get(string culture)
        {
            if (!i18NMessagesCache.ContainsKey(culture))
            {
                lock (sync)
                {
                    if (!i18NMessagesCache.ContainsKey(culture))
                    {
                        i18NMessagesCache[culture] = parser.Parse(Path.Combine(rootPath, "locale", culture, "messages.po"));
                    }
                }
            }
            return i18NMessagesCache[culture];
        }

        public void Reset(string[] changedCultures)
        {
            lock (sync)
            {
                changedCultures.ToList().ForEach(culture => i18NMessagesCache.Remove(culture));
            }
        }
    }
}