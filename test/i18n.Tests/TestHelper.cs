using System;
using System.IO;

namespace i18n.Tests
{
    public class TestBase
    {
        protected string GetRuntimePath()
        {
            var codeBase = typeof(TestBase).Assembly.CodeBase;
            return Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(codeBase).Path));
        } 
    }
}