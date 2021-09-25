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
    public class LeetSpeakController : Controller
    {
        private readonly ILeetSpeakService _leetSpeakService;

        public LeetSpeakController(ILeetSpeakService leetSpeakService)
        {
            _leetSpeakService = leetSpeakService;
        }
        [HttpPost]
        public async Task<IActionResult> ConvertToLeetSpeak([FromBody] LeetSpeakInputDto dto)
        {
            LeetSpeakOutputDto outputDto = null;
            if (dto != null || dto.Text != null) 
            {
                outputDto = await _leetSpeakService.ConvertToLeetSpeak(dto);
            }
            
            return PartialView("_ConvertToLeetSpeak", outputDto);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
