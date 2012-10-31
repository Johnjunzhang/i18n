using System.Collections.Generic;
using NUnit.Framework;

namespace i18n.Tests
{
    [TestFixture]
    public class PostBuildTaskTests
    {
        [Test]
        public void Can_process_message_template()
        {
            const string path = ".";
            var task = new PostBuildTask();
            var fileTypeAllowed = new List<string>(){"csv"};
            task.Execute(path, fileTypeAllowed);
        }
    }
}
