using NUnit.Framework;

namespace i18n.Tests
{
    [TestFixture]
    public class IoCContainerTest
    {
        [Test]
        public void should_return_object_instance_given_correct_key()
        {
            var iocContainer = new IocContainer();
            iocContainer.Register<ILocalizingService>(l=> new LocalizingService());
            var localizingService = iocContainer.Resolve<ILocalizingService>();
            Assert.NotNull(localizingService);
            Assert.AreEqual(typeof (LocalizingService), localizingService.GetType());
        }
    }
}