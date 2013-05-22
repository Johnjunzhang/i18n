using System;
using System.Collections.Generic;
using System.Linq;

namespace i18n.Core
{
    internal class GenericCache<T>
    {
        private readonly IDictionary<string, T> i18NMessagesCache = 
            new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);

        private readonly object sync = new object();

        public T Get(string culture, Func<T> load)
        {
            if (!i18NMessagesCache.ContainsKey(culture))
            {
                lock (sync)
                {
                    if (!i18NMessagesCache.ContainsKey(culture))
                    {
                        i18NMessagesCache[culture] = load();
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