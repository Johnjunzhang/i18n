using System.Web.Hosting;
using i18n.Core;

namespace i18n
{
    public class DependencyResolver
    {
        public static IocContainer Container { get; set; }

        static DependencyResolver()
        {
            Container = new IocContainer();
            Container.Register(i => new I18NMessagesRepository(ApplicationPathProvider.RootPath));
            Container.Register<ILocalizingService>(p => new LocalizingService(Container.Resolve<I18NMessagesRepository>()));
        }

        public static IHtmlStringFormatter HtmlStringFormatter
        {
            get { return Container.Resolve<IHtmlStringFormatter>(); }
        }
        public static ILocalizingService LocalizingService
        {
            get { return Container.Resolve<ILocalizingService>(); }
        }

        public static I18NMessagesRepository I18NMessagesRepository
        {
            get { return Container.Resolve<I18NMessagesRepository>(); }
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