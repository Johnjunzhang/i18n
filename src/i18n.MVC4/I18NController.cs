using System.Collections.Generic;
using System.Web.Mvc;
using i18n.Core.Models;

namespace i18n
{
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

        public virtual string _(string text)
        {
            return _session.GetText(HttpContext, text);
        }

        public virtual IList<I18NMessage> _All()
        {
            return _session.GetAll(HttpContext);
        }
    }
}