using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using Xunit;
using i18n.Core;
using i18n.Core.Models;
using i18n.Core.PoParsers;

namespace i18n.Tests
{
    public class I18NMessagesCacheFacts
    {
        [Fact]
        public void should_read_from_cache_when_no_update_in_po_file()
        {
            var parser = new Mock<IPoFileParser>();
            var expectedI18NMessages = new[] { new I18NMessage("translation key", "translation key") };
            var localeEnUsMessagesPo = Path.Combine(TestHelper.GetRuntimePath(),"locale\\en-US\\messages.po");
            parser.Setup(p => p.Parse(localeEnUsMessagesPo)).Returns(expectedI18NMessages.ToDictionary(d=> d.MsgId));

            var i18NMessagesCache = new GenericCache<IDictionary<string, I18NMessage>>();
            var actualI18NMessages = i18NMessagesCache.Get("en-US", () => parser.Object.Parse(localeEnUsMessagesPo));

            VerifyResult(expectedI18NMessages, actualI18NMessages);
        }

        [Fact]
        public void should_reset_cache_when_po_file_changed()
        {
             var parser = new Mock<IPoFileParser>();
             var expectedI18NMessages = new[] { new I18NMessage("translation key", "translation key") };
            var localeEnUsMessagesPo = Path.Combine(TestHelper.GetRuntimePath(),"locale\\en-US\\messages.po");
            parser.Setup(p => p.Parse(localeEnUsMessagesPo)).Returns(expectedI18NMessages.ToDictionary(d => d.MsgId));

            var i18NMessagesCache = new GenericCache<IDictionary<string, I18NMessage>>();

            var actualI18NMessages = i18NMessagesCache.Get("en-US", () => parser.Object.Parse(localeEnUsMessagesPo));

            VerifyResult(expectedI18NMessages, actualI18NMessages);

            expectedI18NMessages = new[] { new I18NMessage("translation key", "new translation") };
            parser.Setup(p => p.Parse(localeEnUsMessagesPo)).Returns(expectedI18NMessages.ToDictionary(d => d.MsgId));

            string[] changedCultures = {"en-US"};
            i18NMessagesCache.Reset(changedCultures);

            actualI18NMessages = i18NMessagesCache.Get("en-US", () => parser.Object.Parse(localeEnUsMessagesPo));
            VerifyResult(expectedI18NMessages, actualI18NMessages);
        }

        [Fact]
        public void should_return_correct_translation_value_given_correct_key()
        {
            var parser = new Mock<IPoFileParser>();
            var expectedI18NMessages = new[] { new I18NMessage("translation key", "translation vlaue") };
            var localeEnUsMessagesPo = Path.Combine(TestHelper.GetRuntimePath(), "locale\\en-US\\messages.po");
            parser.Setup(p =>p.Parse(localeEnUsMessagesPo)).Returns(expectedI18NMessages.ToDictionary(d => d.MsgId));

            var i18NMessagesCache = new GenericCache<IDictionary<string, I18NMessage>>();
            var result = new LocalizingService(i18NMessagesCache, TestHelper.GetRuntimePath(), parser.Object)
                .GetText("translation key", new []{"en-US"});

            Assert.Equal("translation vlaue", result);
        }

        private static void VerifyResult(IEnumerable<I18NMessage> expectedI18NMessages, IDictionary<string,I18NMessage> result)
        {
            foreach (var message in expectedI18NMessages)
            {
                Assert.Equal(message.MsgStr, result[message.MsgId].MsgStr);
            }
        }
    }
}