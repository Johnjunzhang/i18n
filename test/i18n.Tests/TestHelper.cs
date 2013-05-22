using System;
using System.IO;

namespace i18n.Tests
{
    public static class TestHelper
    {
        public static string GetRuntimePath()
        {
            var codeBase = typeof(TestHelper).Assembly.CodeBase;
            return Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(codeBase).Path));
        } 
        
    }
}