using System;
using System.Globalization;
using Xunit;

namespace i18n.Tests
{
    public class Facts
    {
        [Fact]
        public void FactMethodName()
        {
            Console.WriteLine(CultureInfo.CreateSpecificCulture("zhz").Name);
            
        } 
    }
}