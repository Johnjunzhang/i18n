using System;
using i18n.Core.PoParsers;

namespace i18n.Core
{
    public class I18NFactory : IDisposable
    {
        private static Lazy<I18NFactory> instance;
        private readonly I18NMessagesCache i18NMessagesCache;
        private readonly PoFileWatcher poFileWatcher;

        private I18NFactory(string localePath)
        {
            poFileWatcher = new PoFileWatcher(localePath);
            i18NMessagesCache = new I18NMessagesCache(new I18NPoFileParser(), localePath);
            poFileWatcher.OnChange += (o, e) => i18NMessagesCache.Reset(new ChangeListParser(e.ChangeList).GetChangedCultures());
        }

        public static void Init(string localePath)
        {
            instance = new Lazy<I18NFactory>(() => new I18NFactory(localePath), true);
        }

        public static I18NFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("Please init first. ");
                }
                return instance.Value;
            }
        }

        public ILocalizingService Create()
        {
            return new LocalizingService(new I18NMessagesRepository(i18NMessagesCache));
        }

        public void Dispose()
        {
            poFileWatcher.Dispose();
        }
    }
}