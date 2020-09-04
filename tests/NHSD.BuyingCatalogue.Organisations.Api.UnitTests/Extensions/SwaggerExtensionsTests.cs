﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Extensions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class SwaggerExtensionsTests
    {
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void AddSwaggerDocumentation_ParameterIsNull_ThrowsArgumentNullException(bool hasServiceCollection, bool hasConf)
        {
            var serviceCollection = new Mock<IServiceCollection>();
            var configuration = new Mock<IConfiguration>();

            Assert.Throws<ArgumentNullException>(() =>
                (hasServiceCollection ? serviceCollection.Object : null).AddSwaggerDocumentation(hasConf ? configuration.Object : null));
        }

        [Test]
        public void UseSwaggerDocumentation_ParameterIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SwaggerExtensions.UseSwaggerDocumentation(null));
        }
    }
}