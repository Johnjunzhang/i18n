using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using i18n.Core.Models;

namespace i18n
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddModelError(this ModelStateDictionary dictionary, string key, IHtmlString errorMessage)
        {
            dictionary.AddModelError(key, errorMessage.ToHtmlString());
        }
    }

    /// <summary>
    /// A base controller providing an alias for localizable resources
    /// </summary>
    public abstract class I18NController : Controller, ILocalizing
    {
        private readonly I18NSession _session;

        protected I18NController()
        {
            _session = new I18NSession();
        }

        public virtual IHtmlString _(string text)
        {
            return new MvcHtmlString(_session.GetText(HttpContext, text));
        }

        public virtual IList<I18NMessage> _All()
        {
            return _session.GetAll(HttpContext);
        }
    }
}