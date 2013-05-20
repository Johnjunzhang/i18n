using Xunit;
using i18n.Core.PoParsers;

namespace i18n.Tests.Messages
{
    public class ChangeListParserFacts
    {
        [Fact]
        public void should_return_correct_changed_culture()
        {
            var changeList = new[] { "locale/en-US/messages.po", "locale/en/messages.po" };

            var changedCultures = new ChangeListParser(changeList).GetChangedCultures();

            Assert.Equal(2, changedCultures.Length);
            Assert.Equal("en-US", changedCultures[0]);
            Assert.Equal("en", changedCultures[1]);
        }
    }
}