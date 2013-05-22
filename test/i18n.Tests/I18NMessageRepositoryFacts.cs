using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xunit;
using i18n.Core;
using i18n.Core.Models;

namespace i18n.Tests
{
    public class I18NMessageRepositoryFacts
    {
        [Fact]
        public void should_return_keys_when_no_translation()
        {
            var repository = new I18NMessagesRepository(GetRuntimePath());
            IDictionary<string, I18NMessage> result = repository.Get("en-US");

            Assert.Equal("hasTranslation", result["hasTranslation"].MsgId);
            Assert.NotEmpty(result["hasTranslation"].MsgStr);

            Assert.Equal("notTranslated", result["notTranslated"].MsgId);
            Assert.Equal("notTranslated", result["notTranslated"].MsgStr);
        }

        private string GetRuntimePath()
        {
            var codeBase = typeof(I18NMessageRepositoryFacts).Assembly.CodeBase;
            var uriBuilder = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uriBuilder.Path);
            return Path.GetDirectoryName(path);        
        }
    }
}