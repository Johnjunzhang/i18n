using System;
using System.Collections.Generic;
using i18n.Core.Models;
using i18n.Core.PoParsers;

namespace i18n.Core
{
    public class I18NFactory 
    {
        private static Lazy<ILocalizingService> defaultService;
        private static volatile ILocalizingService localizingService;

        public static void Init(string localePath)
        {
            defaultService = new Lazy<ILocalizingService>(() =>
                {
                    var poFileWatcher = new PoFileWatcher(localePath);
                    var genericCache = new GenericCache<IDictionary<string, I18NMessage>>();
                    poFileWatcher.OnChange += (o, e) => genericCache.Reset(new ChangeListParser(e.ChangeList).GetChangedCultures());
                    return new LocalizingService(genericCache, localePath, new I18NPoFileParser());
                }, true);
        }

        public static ILocalizingService Default
        {
            get
            {
                if (localizingService == null)
                {
                    if (defaultService == null)
                    {
                        throw new Exception("Please init first. ");
                    }
                    localizingService = defaultService.Value;                    
                }
                return localizingService;
            }
            set
            {
                localizingService = value;
            }
        }
    }
}