using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeetSpeakTranslator.Controllers;
using LeetSpeakTranslator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using LeetSpeakTranslator.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using AutoMapper;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using LeetSpeakTranslator;

namespace LeetSpeakTranslatorUnitTests
{
    [TestClass]
    public class LeetSpeakControllerTests
    {
        private readonly ILeetSpeakService _leetSpeakService;
        private readonly IConfiguration _configuration;
        public LeetSpeakControllerTests()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new LeetSpeakMappingProfile());
            });

            var mapper = mockMapper.CreateMapper();
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            _configuration = BuildConfiguration(directory);
            var services = new ServiceCollection();

            var mapperMock = new Mock<IMapper>();
            TranslatorAPIs translatorAPIs = new();
            _configuration.GetSection("TranslatorAPIs").Bind(translatorAPIs);
            services.AddSingleton(mapper);
            services.AddSingleton(translatorAPIs);
            services.AddScoped<ILeetSpeakService, LeetSpeakService>();
            services.AddLogging();
            var serviceProvider = services.BuildServiceProvider();

            _leetSpeakService = serviceProvider.GetService<ILeetSpeakService>();
            
        }
        [TestMethod]
        public void TestIndexViewName_ShouldReturnCorrectViewName()
        {
            //arrange
            var controller = new LeetSpeakController(_leetSpeakService);

            //act
            var result = controller.Index() as ViewResult;

            //assert
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public async Task TestConvertToLeetSpeakViewName_ShouldReturnCorrectViewName()
        {
            //arrange
            var controller = new LeetSpeakController(_leetSpeakService);
            var dto = new LeetSpeakInputDto()
            {
                Text = "example string"
            };

            //act
            var result = await controller.ConvertToLeetSpeak(dto) as PartialViewResult;

            //assert
            Assert.AreEqual("_ConvertToLeetSpeak", result.ViewName);
        }
        [TestMethod]
        public async Task TestConvertToLeetSpeakNormalInput_ShouldReturnCode429OrConvertedText()
        {
            //arrange
            var controller = new LeetSpeakController(_leetSpeakService);
            var dto = new LeetSpeakInputDto()
            {
                Text = "example string"
            };

            //act
            var result = await controller.ConvertToLeetSpeak(dto) as PartialViewResult;

            //assert
            var resultModel = (LeetSpeakOutputDto)result.Model;
            if (string.IsNullOrEmpty(resultModel.ConvertedText))
            {
                Assert.AreEqual("429", resultModel.Code);
            }
            else
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Newtonsoft.Json.JsonReaderException))]
        public async Task TestConvertToLeetSpeakNonAlphanumericInput_ShouldThrowJSONexception()
        {
            //arrange
            var controller = new LeetSpeakController(_leetSpeakService);
            var dto = new LeetSpeakInputDto()
            {
                Text = "~'!@#$%^&*()_-+={}[]|\\:;<,>.?/"
            };

            //act
            var result = await controller.ConvertToLeetSpeak(dto) as PartialViewResult;
            Assert.Fail();
            //assert
        }

        [TestMethod]
        public async Task TestConvertToLeetSpeakEmptyInput_ShouldReturnCode429OrEmptyConvertedText()
        {
            var controller = new LeetSpeakController(_leetSpeakService);
            var dto = new LeetSpeakInputDto()
            {
                Text = ""
            };

            //act
            var result = await controller.ConvertToLeetSpeak(dto) as PartialViewResult;

            //assert
            var resultModel = (LeetSpeakOutputDto)result.Model;
            if (string.IsNullOrEmpty(resultModel.Code))
            {
                Assert.AreEqual(resultModel.ConvertedText, "");
            }
            else
            {
                Assert.AreEqual("429", resultModel.Code);
            }
        }

        public IConfigurationRoot BuildConfiguration(string testDirectory)
        {
            return new ConfigurationBuilder()
                .SetBasePath(testDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }
    }
}
