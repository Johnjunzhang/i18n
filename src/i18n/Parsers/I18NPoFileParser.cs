using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace i18n.Parsers
{
    public class I18NPoFileParser
    {
        public IList<I18NMessage> Parse(string path)
        {
            var i18NMessages = new List<I18NMessage>();
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var fs = new StreamReader(fileStream, Encoding.Default))
                {
                    string line = fs.ReadLine();
                    while (line != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith("msgid"))
                        {
                            var msgId = ParseTranslation(ref line, fs, "msgid");
                            var msgStr = ParseTranslation(ref line, fs, "msgstr");
                            if (string.IsNullOrEmpty(msgStr))
                            {
                                msgStr = msgId;
                            }
                            i18NMessages.Add(new I18NMessage(msgId, msgStr));    
                        }
                        else
                        {
                            line = fs.ReadLine();
                        }
                        
                    }
                }
            }
            return i18NMessages;
        }

        private static string ParseTranslation(ref string line, StreamReader fs, string key)
        {
            var values = new List<string>();
            var keyPattern = string.Format("(?:^{0}\\s*)\"(?<{0}>.*)\"", key);
            if (Regex.IsMatch(line, keyPattern))
            {
                values.Add(Regex.Match(line, keyPattern).Groups[key].Value);
                while ((line = fs.ReadLine()) != null && line.StartsWith("\""))
                {
                    values.Add(line.Trim('"'));
                }
            }
            return string.Join("", values);
        }
    }
}