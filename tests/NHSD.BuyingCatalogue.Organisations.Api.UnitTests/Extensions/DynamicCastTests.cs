using System.Collections.Generic;
using System.Dynamic;
using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.Extensions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DynamicCastTests
    {
        [Test]
        public void DynamicObject_HoldingStringValue_TryGetPropertyAs_String_ReturnsString()
        {
            dynamic expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;
            dictionary.Add("foo", "bar");

            string expected = DynamicCast.GetPropertyOrDefault<string>(expando, "foo");
            expected.Should().BeEquivalentTo("bar");
        }

        [Test]
        public void DynamicObject_HoldingIntValue_TryGetPropertyAs_Int_ReturnsInt()
        {
            dynamic expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;
            dictionary.Add("foo", 1);

            int expected = DynamicCast.GetPropertyOrDefault<int>(expando, "foo");
            expected.Should().Be(1);
        }

        [Test]
        public void DynamicObject_HoldingObjectValue_TryGetPropertyAs_Object_ReturnsObject()
        {
            dynamic expando = new ExpandoObject();
            var testObj = new { Foo = "bar", Baz = "bazz" };
            var dictionary = (IDictionary<string, object>)expando;
            dictionary.Add("foo", testObj);

            object expected = DynamicCast.GetPropertyOrDefault<object>(expando, "foo");
            expected.Should().BeEquivalentTo(testObj);
        }

        [Test]
        public void DynamicObject_HoldingDynamicObjectValue_TryGetPropertyAs_DynamicObject_ReturnsDynamicObject()
        {
            dynamic expando = new ExpandoObject();
            dynamic testObj = new { Foo = "bar", Baz = "bazz" };
            var dictionary = (IDictionary<string, object>)expando;
            dictionary.Add("foo", testObj);

            dynamic expected = DynamicCast.GetPropertyOrDefault<dynamic>(expando, "foo");

            string foo = expected.Foo;
            string baz = expected.Baz;

            foo.Should().BeEquivalentTo("bar");
            baz.Should().BeEquivalentTo("bazz");
        }

        [Test]
        public void DynamicObject_HoldingNull_TryGetProperty_ReturnsNull()
        {
            dynamic expando = new ExpandoObject();

            object expected = DynamicCast.GetPropertyOrDefault<object>(expando, "foo");

            expected.Should().BeNull();
        }
    }
}
