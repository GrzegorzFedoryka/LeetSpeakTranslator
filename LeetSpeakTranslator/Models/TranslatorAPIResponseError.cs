using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeetSpeakTranslator.Models
{
    public class TranslatorAPIResponseError
    {
        public class APIError
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }

        public APIError Error { get; set; }
    }
    
}
