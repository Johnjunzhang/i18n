using Xunit;

namespace i18n.Tests
{
    public class IoCContainerTest
    {
        [Fact]
        public void should_return_object_instance_given_correct_key()
        {
            var iocContainer = new IocContainer();
            iocContainer.Register<ILocalizingService>(l=> new LocalizingService());
            var localizingService = iocContainer.Resolve<ILocalizingService>();
            Assert.NotNull(localizingService);
            Assert.Equal(typeof (LocalizingService), localizingService.GetType());
        }
    }
}