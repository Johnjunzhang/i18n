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
            I18NFactory.Init(TestHelper.GetRuntimePath());
            var repository = I18NFactory.Default;
            var result = repository.GetAll(new []{"en-US"}).ToDictionary(m => m.MsgId);

            Assert.Equal("hasTranslation", result["hasTranslation"].MsgId);
            Assert.NotEmpty(result["hasTranslation"].MsgStr);

            Assert.Equal("notTranslated", result["notTranslated"].MsgId);
            Assert.Equal("notTranslated", result["notTranslated"].MsgStr);
        }
    }
}