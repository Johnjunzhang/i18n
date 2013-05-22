using System.Web;
using i18n.Core;

namespace i18n.Web
{
    public static class HttpContextLangExtensions
    {
        private const string SessionKey = "po:language";
        public static object GetLang(this HttpContextBase context)
        {
            return context.Session != null ? context.Session[SessionKey] : null;
        }

        public static void SetLang(this HttpContextBase context, string language)
        {
            if (context.Session != null)
            {
                context.Session[SessionKey] = language;
            }
        }

        public static string GetText(this HttpContextBase context, string text)
        {
            return I18NFactory.Instance.Create().GetText(text, GetLanguages(context));
        }
        
        public static string _(this HttpContextBase context, string text)
        {
            return GetText(context, text);
        }

        private static string[] GetLanguages(HttpContextBase context)
        {
            var sessionValue = context.GetLang();
            return sessionValue == null
                             ? context.Request.UserLanguages
                             : new[] { sessionValue.ToString() };
        }

    }
}