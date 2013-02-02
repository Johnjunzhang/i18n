﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace i18n.Parsers
{
    public class I18NPoFileParser
    {
        private const string TRANSLATION_KEY = "msgid";
        private const string TRANSLATION = "msgstr";

        public IList<I18NMessage> Parse(string path)
        {
            var i18NMessages = new List<I18NMessage>();
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var fs = new StreamReader(fileStream, Encoding.Default))
                {
                    var line = fs.ReadLine();
                    while (line != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith(TRANSLATION_KEY))
                        {
                            var result = new[] {TRANSLATION_KEY, TRANSLATION}.ToList().Select(s => ParseTranslation(ref line, fs, s)).ToArray();
                            i18NMessages.Add(new I18NMessage(result[0], result[1]));    
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