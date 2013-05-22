using System.Web.Mvc;

namespace i18n.Web
{
    public abstract class I18NWebViewPage<T> : WebViewPage<T>
    {   
        public string _(string text)
        {
            return Context.GetText(text);
        }
    }

    public abstract class I18NWebViewPage : WebViewPage
    {
        public string _(string text)
        {
            return Context.GetText(text);
        }
    }
}