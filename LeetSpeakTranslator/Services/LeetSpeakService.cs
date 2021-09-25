using AutoMapper;
using LeetSpeakTranslator.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LeetSpeakTranslator.Services
{
    public interface ILeetSpeakService
    {
        Task<LeetSpeakOutputDto> ConvertToLeetSpeak(LeetSpeakInputDto sto);
    }

    public class LeetSpeakService : ILeetSpeakService
    {
        private readonly TranslatorAPIs _translatorApis;
        private readonly IMapper _mapper;
        private readonly ILogger<LeetSpeakService> _logger;

        public LeetSpeakService(TranslatorAPIs translatorApis, IMapper mapper, ILogger<LeetSpeakService> logger)
        {
            _translatorApis = translatorApis;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<LeetSpeakOutputDto> ConvertToLeetSpeak(LeetSpeakInputDto dto)
        {
            _logger.LogInformation($"Leet speak translator input: {dto.Text}");
            string apiUrl = _translatorApis.LeetSpeak;
            string responseStr = "";


                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("text", dto.Text)
                });

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    responseStr = await response.Content.ReadAsStringAsync();
                }

                var leetSpeak = JsonConvert.DeserializeObject<LeetSpeakResponse>(responseStr);
            
            LeetSpeakOutputDto outputDto = null;
            if(leetSpeak != null)
            {
                outputDto = _mapper.Map<LeetSpeakOutputDto>(leetSpeak);
            }
            if(outputDto != null && outputDto.ConvertedText == null && outputDto.InputText == null)
            {
                var leetSpeakError = JsonConvert.DeserializeObject<LeetSpeakResponseError>(responseStr);
                outputDto = _mapper.Map<LeetSpeakOutputDto>(leetSpeakError);
                outputDto.InputText = dto.Text;
            }
            if (outputDto != null)
            {
                _logger.LogInformation($"Leet speak translator output: \r\n" +
                    $"Input text: {outputDto.InputText}\r\n" +
                    $"Converted text: {outputDto.ConvertedText}\r\n" +
                    $"Error message: {outputDto.ErrorMessage}");
            }
            else
            {
                _logger.LogWarning("Leet speak output is null");
            }


            return outputDto;
        }
    }
}
