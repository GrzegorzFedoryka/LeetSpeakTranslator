using AutoMapper;
using LeetSpeakTranslator.Models;
using LeetSpeakTranslator.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeetSpeakTranslator.Controllers
{
    [Route("")]
    [Route("[controller]")]
    public class TranslatorController : Controller
    {
        private readonly ITranslatorService _translatorService;

        public TranslatorController(ITranslatorService translatorService)
        {
            _translatorService = translatorService;
        }
        [HttpPost("ConvertText/{apiName}")]
        public async Task<IActionResult> ConvertText([FromRoute]string apiName, [FromBody]TranslatorInputDto dto)
        {
            TranslatorOutputDto outputDto = null;
            if (dto != null || dto.Text != null)
            {
                outputDto = await _translatorService.ConvertText(apiName, dto);
            }
            return PartialView("_ConvertText", outputDto);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
