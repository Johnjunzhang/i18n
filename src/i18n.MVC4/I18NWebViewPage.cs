using System.Web.Mvc;

namespace i18n
{
    public abstract class I18NWebViewPage<T> : WebViewPage<T>, ILocalizing
    {
        private readonly I18NSession _session;

        protected I18NWebViewPage()
        {
            _session = new I18NSession();
        }
        
        public string _(string text)
        {
            return _session.GetText(Context, text);
        }
    }

    public abstract class I18NWebViewPage : WebViewPage, ILocalizing
    {
        private readonly I18NSession _session;

        protected I18NWebViewPage()
        {
            _session = new I18NSession();
        }

        public string _(string text)
        {
            return _session.GetText(Context, text);
        }
    }
}