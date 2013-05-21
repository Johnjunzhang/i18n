using System.Web.Hosting;
using i18n.Core;
using i18n.Core.PoParsers;

namespace i18n
{
    public class I18NSessionFactory
    {
        private static readonly I18NMessagesCache i18NMessagesCache;
        static I18NSessionFactory()
        {
            var rootPath = ApplicationPathProvider.RootPath;
            var poFileWatcher = new PoFileWatcher(rootPath);
            i18NMessagesCache = new I18NMessagesCache(new I18NPoFileParser(), rootPath);
            poFileWatcher.OnChange += (o, e) => i18NMessagesCache.Reset(new ChangeListParser(e.ChangeList).GetChangedCultures());
        }

        public static I18NMessagesRepository CreateRepository()
        {
            return new I18NMessagesRepository(i18NMessagesCache);
        }
    }


    public class ApplicationPathProvider
    {
        private static string defaultPath = string.Empty;

        public static string RootPath
        {
            get { return HostingEnvironment.ApplicationPhysicalPath ?? defaultPath; }
        }

        public static void Using(string userDefinedPath )
        {
            defaultPath = userDefinedPath;
        }
    }
}