using Xunit;
using i18n.Core;
using System.Linq;

namespace i18n.Tests
{
    public class I18NMessageRepositoryFacts
    {
        [Fact]
        public void should_return_keys_when_no_translation()
        {
            var i18NFactory = new I18NFactory(TestHelper.GetRuntimePath());

            var repository = i18NFactory.Create();
            var result = repository.GetAll(new []{"en-US"}).ToDictionary(m => m.MsgId);

            Assert.Equal("hasTranslation", result["hasTranslation"].MsgId);
            Assert.NotEmpty(result["hasTranslation"].MsgStr);

            Assert.Equal("notTranslated", result["notTranslated"].MsgId);
            Assert.Equal("notTranslated", result["notTranslated"].MsgStr);
        }
    }
}