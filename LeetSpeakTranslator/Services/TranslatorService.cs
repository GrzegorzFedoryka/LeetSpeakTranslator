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
    public interface ITranslatorService
    {
        Task<TranslatorOutputDto> ConvertText(string apiName, TranslatorInputDto sto);
    }

    public class TranslatorService : ITranslatorService
    {
        private readonly TranslatorAPI _translatorApi;
        private readonly IMapper _mapper;
        private readonly ILogger<TranslatorService> _logger;

        public TranslatorService(TranslatorAPI translatorApi, IMapper mapper, ILogger<TranslatorService> logger)
        {
            _translatorApi = translatorApi;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<TranslatorOutputDto> ConvertText(string apiName, TranslatorInputDto dto) 
        {
            _logger.LogInformation($"Translator input: {dto.Text}" +
                $"Chosen translator: " + apiName);
            TranslatorOutputDto outputDto = null;
            if (!Enum.IsDefined(typeof(TranslatorsEnum), apiName))
            {
                outputDto = new TranslatorOutputDto();
                outputDto.ErrorMessage = "Api doesn't exist or is not supported";
                return outputDto;
            }
            string apiUrl = _translatorApi.Api + apiName;
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

                var convertedText = JsonConvert.DeserializeObject<TranslatorAPIResponse>(responseStr);
            
            
            if(convertedText != null)
            {
                outputDto = _mapper.Map<TranslatorOutputDto>(convertedText);
            }
            if(outputDto != null && outputDto.ConvertedText == null && outputDto.InputText == null)
            {
                var convertedTextError = JsonConvert.DeserializeObject<TranslatorAPIResponseError>(responseStr);
                outputDto = _mapper.Map<TranslatorOutputDto>(convertedTextError);
                outputDto.InputText = dto.Text;
            }
            if (outputDto != null)
            {
                _logger.LogInformation($"Translator output: \r\n" +
                    $"Input text: {outputDto.InputText}\r\n" +
                    $"Converted text: {outputDto.ConvertedText}\r\n" +
                    $"Error message: {outputDto.ErrorMessage}");
            }
            else
            {
                _logger.LogWarning("Translator output is null");
            }


            return outputDto;
        }
    }
}
