using System.Linq;

namespace i18n.Core.Parsers
{
    public class ChangeListParser
    {
        private readonly string[] changeList;

        public ChangeListParser(string[] changeList)
        {
            this.changeList = changeList;
        }

        public string[] GetChangedCultures()
        {
            return changeList.ToList().Select(GetCulture).ToArray();
        }

        private string GetCulture(string path)
        {
            char[] delimeter = { '\\', '/' };
            var directionaries = path.Split(delimeter);
            return directionaries[directionaries.Length - 2];
        }
    }
}