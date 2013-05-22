using System;
using System.Collections.Generic;
using i18n.Core.Models;
using i18n.Core.PoParsers;

namespace i18n.Core
{
    public class I18NFactory : IDisposable
    {
        private readonly string localePath;
        private static Lazy<I18NFactory> @default;
        private readonly GenericCache<IDictionary<string, I18NMessage>> genericCache;
        private readonly PoFileWatcher poFileWatcher;

        public I18NFactory(string localePath)
        {
            this.localePath = localePath;
            poFileWatcher = new PoFileWatcher(localePath);
            genericCache = new GenericCache<IDictionary<string, I18NMessage>>();
            poFileWatcher.OnChange += (o, e) => genericCache.Reset(new ChangeListParser(e.ChangeList).GetChangedCultures());
        }

        public static void Init(string localePath)
        {
            @default = new Lazy<I18NFactory>(() => new I18NFactory(localePath), true);
        }

        public static ILocalizingService Default
        {
            get
            {
                if (@default == null)
                {
                    throw new Exception("Please init first. ");
                }
                return @default.Value.Create();
            }
        }

        public ILocalizingService Create()
        {
            return new LocalizingService(genericCache, localePath, new I18NPoFileParser());
        }

        public void Dispose()
        {
            poFileWatcher.Dispose();
        }
    }
}