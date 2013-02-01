using System;
using System.IO;
using Xunit;
using i18n.Parsers;

namespace i18n.Tests.Parsers
{
    public class I18NPoFileParserFacts
    {
        [Fact]
        public void should_parse_po_file_correctly()
        {
            var parser = new I18NPoFileParser();
            var path = GetDir("locale/en-US/messages.po");
            var result = parser.Parse(path);

            var i18NMessage = new I18NMessage("Afghanistan", "Afghanistan");
            const string longMessage = @"Depending on your answers, myVisas will be automatically tailored to your situation and filter sections that you are only required th complete, by either adding or removing sections and questions from the questionnaire.";
            var longI18NMessage = new I18NMessage(longMessage, longMessage);

            Assert.Equal(7, result.Count);

            Assert.True(result.Contains(i18NMessage));
            Assert.True(result.Contains(longI18NMessage));
        }

        private string GetDir(string relativePath = "")
        {
            string codeBase = typeof (I18NPoFileParserFacts).Assembly.CodeBase;
            var uriBuilder = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uriBuilder.Path);
            string directoryName = Path.GetDirectoryName(path);
            return Path.Combine(directoryName, relativePath);
        }
    }
}