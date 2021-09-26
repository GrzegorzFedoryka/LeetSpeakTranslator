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
    public class TranslatorControllerTests
    {
        private readonly ITranslatorService _translatorService;
        private readonly IConfiguration _configuration;
        public TranslatorControllerTests()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TranslatorMappingProfile());
            });

            var mapper = mockMapper.CreateMapper();
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            _configuration = BuildConfiguration(directory);
            var services = new ServiceCollection();

            var mapperMock = new Mock<IMapper>();
            TranslatorAPI translatorAPI = new();
            _configuration.GetSection("TranslatorAPI").Bind(translatorAPI);
            services.AddSingleton(mapper);
            services.AddSingleton(translatorAPI);
            services.AddScoped<ITranslatorService, TranslatorService>();
            services.AddLogging();
            var serviceProvider = services.BuildServiceProvider();

            _translatorService = serviceProvider.GetService<ITranslatorService>();
            
        }
        [TestMethod]
        public void TestIndexViewName_ShouldReturnCorrectViewName()
        {
            //arrange
            var controller = new TranslatorController(_translatorService);

            //act
            var result = controller.Index() as ViewResult;

            //assert
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public async Task TestConvertTextViewName_ShouldReturnCorrectViewName()
        {
            //arrange
            var controller = new TranslatorController(_translatorService);
            var dto = new TranslatorInputDto()
            {
                Text = "example string"
            };

            //act
            var result = await controller.ConvertText("leetspeak", dto) as PartialViewResult;

            //assert
            Assert.AreEqual("_ConvertText", result.ViewName);
        }
        [TestMethod]
        public async Task TestConvertTextNormalInput_ShouldReturnCode429OrConvertedText()
        {
            //arrange
            var controller = new TranslatorController(_translatorService);
            var dto = new TranslatorInputDto()
            {
                Text = "example string"
            };

            //act
            var result = await controller.ConvertText("leetspeak", dto) as PartialViewResult;

            //assert
            var resultModel = (TranslatorOutputDto)result.Model;
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
        public async Task TestConvertTextNonAlphanumericInput_ShouldThrowJSONexception()
        {
            //arrange
            var controller = new TranslatorController(_translatorService);
            var dto = new TranslatorInputDto()
            {
                Text = "~'!@#$%^&*()_-+={}[]|\\:;<,>.?/"
            };

            //act
            var result = await controller.ConvertText("leetspeak", dto) as PartialViewResult;
            Assert.Fail();
            //assert
        }

        [TestMethod]
        public async Task TestConvertTextEmptyInput_ShouldReturnCode429OrEmptyConvertedText()
        {
            var controller = new TranslatorController(_translatorService);
            var dto = new TranslatorInputDto()
            {
                Text = ""
            };

            //act
            var result = await controller.ConvertText("leetspeak", dto) as PartialViewResult;

            //assert
            var resultModel = (TranslatorOutputDto)result.Model;
            if (string.IsNullOrEmpty(resultModel.Code))
            {
                Assert.AreEqual(resultModel.ConvertedText, "");
            }
            else
            {
                Assert.AreEqual("429", resultModel.Code);
            }
        }

        [TestMethod]
        public async Task TestConvertUnsupportedAPI_ShouldReturnErrorJSON()
        {
            //arrange
            var controller = new TranslatorController(_translatorService);
            var dto = new TranslatorInputDto()
            {
                Text = "example string"
            };

            //act
            var result = await controller.ConvertText("yoda", dto) as PartialViewResult;

            //assert
            var resultModel = (TranslatorOutputDto)result.Model;
            Assert.AreEqual(resultModel.ErrorMessage, "Api doesn't exist or is not supported");
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
