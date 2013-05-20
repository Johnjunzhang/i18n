using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using i18n.Core.Models;
using i18n.Core.PoParsers;

namespace i18n.Tests.Messages
{
    public class I18NPoFileParserFacts : TestBase
    {
        private readonly I18NPoFileParser parser;
        const string testPoFileName = "test.po";
        private readonly string poFileRuntimePath;

        public I18NPoFileParserFacts()
        {
            parser = new I18NPoFileParser();
            poFileRuntimePath = Path.Combine(GetRuntimePath(), testPoFileName);
        }

        [Fact]
        public void should_return_correct_msgid_and_msgstr_when_parse_po_file_with_translation()
        {
            const string poFileContent = "#: .\\test.html:3\nmsgid \"translation key\"\nmsgstr \"translation\"";
            CreatePoFile(testPoFileName, poFileContent);

            var result = (IList<I18NMessage>) parser.Parse(poFileRuntimePath).Values.ToList();
            Assert.Equal(1, result.Count);

            var expectedI18NMessages = new[]{new I18NMessage("translation key", "translation")};
            VerifyResult(expectedI18NMessages, result);

            Dispose(poFileRuntimePath);
        }

        [Fact]
        public void should_parse_translation_key_in_multiple_line()
        {
            const string content = @"#: .\\test.html:22
msgid """"
""one line ""
""another line ""
msgstr """"";
            CreatePoFile(testPoFileName, content);

            var result = (IList<I18NMessage>) parser.Parse(poFileRuntimePath).Values.ToList();

            Assert.Equal(1, result.Count);

            var expectedI18NMessages = new[]{new I18NMessage("one line another line ", "")};
            VerifyResult(expectedI18NMessages, result);

            Dispose(poFileRuntimePath);
        }

        [Fact]
        public void should_return_correct_result_when_parse_multiple_translation_block_with_blank_line_inside()
        {
            const string content = @"..\\test.html:15
msgid ""welcome""
msgstr """"

#: ..\\test.html:20
#: ..\\test-another.html:22
msgid ""another.""
msgstr ""another's translation""

#: ..\\test.html:45
#, fuzzy
msgid """"
""For ease of completing this questionnaire, you have to answer some mandatory ""
""questions before starting.""
msgstr """"

";
            CreatePoFile(testPoFileName, content);

            var result = (IList<I18NMessage>) parser.Parse(poFileRuntimePath).Values.ToList();

            Assert.Equal(3, result.Count);

            var expectedI18NMessages = new[]
                {
                    new I18NMessage("welcome", ""), 
                    new I18NMessage("another.", "another's translation"), 
                    new I18NMessage("For ease of completing this questionnaire, you have to answer some mandatory questions before starting.", "")
                };

            VerifyResult(expectedI18NMessages, result);

            Dispose(poFileRuntimePath);
        }

        private static void VerifyResult(I18NMessage[] expectedI18NMessages, IList<I18NMessage> result)
        {
            for (int i = 0; i < expectedI18NMessages.Length; i++)
            {
                Assert.Equal(expectedI18NMessages[i], result[i]);
            }
        }

        private void CreatePoFile(string path, string content)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                fileStream.Write(Encoding.UTF8.GetBytes(content), 0, Encoding.UTF8.GetByteCount(content));
            }
        }

        private void Dispose(string path)
        {
           File.Delete(path);
        }
    }
}