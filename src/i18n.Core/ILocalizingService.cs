using System.Collections.Generic;
using i18n.Core.Models;

namespace i18n.Core
{
    public interface ILocalizingService
    {
        string GetBestAvailableLanguageFrom(string[] languages);
        string GetText(string key, string[] languages);
        IList<I18NMessage> GetAll(string[] languages);
    }
}